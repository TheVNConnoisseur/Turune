using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

internal class SDT
{
    internal class SDTScheme
    {
        public uint Opcodes_Offset { get; set; }
        public uint Unknown1_Count { get; set; }
        public uint Unknown2_Count { get; set; }
        public uint Label_Count { get; set; }
        public uint SDTKey_Offset { get; set; }
        public List<SDT_Label> SDT_Labels { get; set; } = [];   //There must be Label_Count amount
        public byte[] SDTKey { get; set; } = [];    //While the SDT key can be anywhere, the developers just put it after the list of labels (if it exists)
        public List<SDT_Opcode> Opcodes { get; set; } = [];
    }

    /// <summary>
    /// For the jump labels, the game keeps a list of each possible label, and then
    /// next to it the offset relative to the opcode region.
    /// </summary>
    internal class SDT_Label
    {
        public string Name { get; set; } = "";
        public uint Offset { get; set; }
    }

    /// <summary>
    /// Each of the opcodes requires certain parameters, and to avoid having to create
    /// one type of class per opcode, it's better to just accept each parameters as a string.
    /// </summary>
    internal class SDT_Opcode
    {
        public string Name { get; set; } = "";
        public List<string> Parameters { get; set; } = [];
        public uint Offset { get; set; }    //Indicates where the absolute offset opcode starts
    }

    public SDTScheme SdtScheme { get; private set; } = new SDTScheme();

    public void ParseSDT(byte[] inputFile, Encoding encoding)
    {
        int currentOffset = 0;

        //Reading the values from the header
        SdtScheme.Opcodes_Offset = BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
        currentOffset += 4;

        SdtScheme.Unknown1_Count = BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
        currentOffset += 4;

        SdtScheme.Unknown2_Count = BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
        currentOffset += 4;

        SdtScheme.Label_Count = BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
        currentOffset += 4;

        SdtScheme.SDTKey_Offset = BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
        currentOffset += 4;

        if (SdtScheme.Label_Count > 0)
        {
            for (int i = 0; i < SdtScheme.Label_Count; i++)
            {
                string sdt_Name = GetEncrytedString(inputFile, ref currentOffset, encoding);
                uint sdt_Offset = BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
                SDT_Label sdt_label = new()
                {
                    Name = sdt_Name,
                    Offset = sdt_Offset + SdtScheme.Opcodes_Offset //Making the offset absolute, not relative
                };
                currentOffset += 4; //The offset needs to be passed through
                SdtScheme.SDT_Labels.Add(sdt_label);
            }
        }

        while (currentOffset < SdtScheme.Opcodes_Offset)    //Skip the SDT key
            currentOffset++;

        while (currentOffset < inputFile.Length)
            SdtScheme.Opcodes.Add(ParseOpcode(inputFile, ref currentOffset, encoding));
    }
    
    private SDT_Opcode ParseOpcode(byte[] inputFile, ref int currentOffset, Encoding encoding)
    {
        SDT_Opcode opcode = new();

        uint currentOpcode = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
        currentOffset += 2;

        switch (currentOpcode)
        {
            //One parameter is not well understood
            case 0x1:
                {
                    //Checks if the game is in debug mode, otherwise it skips the entire if
                    opcode.Name = "DebugIF";       //In the logs it shows up as "DebugIF{"

                    uint bytesToSkip = BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4)); //Indicates how many bytes to skip from the SDT file if the game detects that is not in debug mode
                    currentOffset += 4;
                    opcode.Parameters.Add(bytesToSkip.ToString());

                    break;
                }
            case 0x2:
                {
                    //Open a MessageBoxA with the given parameter as the lpCaption.
                    opcode.Name = "DebugMessage";

                    opcode.Parameters.Add(GetStringParameter(inputFile, ref currentOffset, encoding));  //The message that will be shown in the MessageBox
                    break;
                }
            case 0x4:
                {
                    opcode.Name = "GetSysModeOn";
                    break;
                }
            
            //File IO
            case 0x500:
                {
                    opcode.Name = "OpenUrlFiles";
                    break;
                }
            case 0x501:
                {
                    opcode.Name = "OpenTextFiles";
                    break;
                }
            case 0x502:
                {
                    opcode.Name = "OpenFoloder";
                    break;
                }
            case 0x503:
                {
                    opcode.Name = "OpenEXE";
                    break;
                }
            
            //Save files
            case 0x600:
                {
                    opcode.Name = "GlobalSave";
                    break;
                }
            case 0x601:
                {
                    opcode.Name = "GlobalLoad";
                    break;
                }
            case 0x602:
                {
                    opcode.Name = "LocalSaveCreate";
                    break;
                }
            case 0x603:
                {
                    opcode.Name = "LocalSave";
                    break;
                }
            case 0x604:
                {
                    opcode.Name = "LocalLoad";
                    break;
                }
            case 0x605:
                {
                    opcode.Name = "SetSaveComment";
                    break;
                }
            case 0x606:
                {
                    opcode.Name = "GetSaveComment";
                    break;
                }
            case 0x607:
                {
                    opcode.Name = "GetSaveTime";
                    break;
                }
            case 0x608:
                {
                    opcode.Name = "GetSaveCG";
                    break;
                }
            case 0x609:
                {
                    opcode.Name = "GetSaveHeadAll";
                    break;
                }
            case 0x60A:
                {
                    opcode.Name = "SaveFileChk";
                    break;
                }
            case 0x60B:
                {
                    opcode.Name = "GetSaveBuffCG";
                    break;
                }
            case 0x60C:
                {
                    opcode.Name = "LocalSaveEx";
                    break;
                }
            case 0x60D:
                {
                    opcode.Name = "GetSaveTextNameOBJ";
                    break;
                }
            case 0x60E:
                {
                    opcode.Name = "GetSaveTextOBJ";
                    break;
                }
            
            //Backlog
            case 0x700:
                {
                    opcode.Name = "StartBakLog";
                    break;
                }
            case 0x701:
                {
                    opcode.Name = "EndBakLog";
                    break;
                }
            case 0x702:
                {
                    opcode.Name = "BakLogName";
                    break;
                }
            case 0x703:
                {
                    opcode.Name = "BakLogText";
                    break;
                }
            case 0x704:
                {
                    opcode.Name = "BakLogKoe";
                    break;
                }
            case 0x705:
                {
                    opcode.Name = "BakLogTxtPosX";
                    break;
                }
            case 0x706:
                {
                    opcode.Name = "BakLogTxtPosY";
                    break;
                }
            case 0x707:
                {
                    opcode.Name = "BakLogTxtLimitLR";
                    break;
                }
            case 0x708:
                {
                    opcode.Name = "BakLogNextLine";
                    break;
                }
            case 0x709:
                {
                    opcode.Name = "BakLogGetTxtPos";
                    break;
                }
            case 0x70A:
                {
                    opcode.Name = "BakLogKazu";
                    break;
                }
            case 0x70B:
                {
                    opcode.Name = "BakLogChk";
                    break;
                }
            case 0x70C:
                {
                    opcode.Name = "BakLogNameChk";
                    break;
                }
            case 0x70D:
                {
                    opcode.Name = "BakLogKoeChk";
                    break;
                }
            case 0x70E:
                {
                    opcode.Name = "BakLogReset";
                    break;
                }
            case 0x70F:
                {
                    opcode.Name = "SetKaigyouToFont";
                    break;
                }
            case 0x710:
                {
                    opcode.Name = "SetKaigyouToPixel";
                    break;
                }
            case 0x711:
                {
                    opcode.Name = "D_BakLogShadow";
                    break;
                }
            case 0x712:
                {
                    opcode.Name = "BakLogONOFF";
                    break;
                }
            
            //Audio
            case 0x800:
                {
                    opcode.Name = "AudioPlay";
                    break;
                }
            case 0x802:
                {
                    opcode.Name = "AudioStop";
                    break;
                }
            case 0x803:
                {
                    opcode.Name = "AudioPlayChk";
                    break;
                }
            case 0x804:
                {
                    opcode.Name = "AudioVolume";
                    break;
                }
            case 0x805:
                {
                    opcode.Name = "AudioRepeat";
                    break;
                }
            case 0x806:
                {
                    opcode.Name = "AudioFadeTime";
                    break;
                }
            case 0x807:
                {
                    opcode.Name = "AudioFadeInTime";
                    break;
                }
            case 0x808:
                {
                    opcode.Name = "AudioFadeOutTime";
                    break;
                }
            case 0x809:
                {
                    opcode.Name = "AudioFadeMode";
                    break;
                }
            case 0x80A:
                {
                    opcode.Name = "AudioCrossFadeINVolume";
                    break;
                }
            case 0x80B:
                {
                    opcode.Name = "AudioSetParmBGM";
                    break;
                }
            case 0x80C:
                {
                    opcode.Name = "AudioSetParmSE";
                    break;
                }
            case 0x80D:
                {
                    opcode.Name = "AudioSetParmKOE";
                    break;
                }
            case 0x80E:
                {
                    opcode.Name = "BgmPlay";
                    break;
                }
            case 0x810:
                {
                    opcode.Name = "BgmStop";
                    break;
                }
            case 0x811:
                {
                    opcode.Name = "KoePlay";
                    break;
                }
            case 0x812:
                {
                    opcode.Name = "KoeStop";
                    break;
                }
            case 0x813:
                {
                    opcode.Name = "KoeVolume";
                    break;
                }
            case 0x814:
                {
                    opcode.Name = "KoeStopR";
                    break;
                }
            case 0x815:
                {
                    opcode.Name = "AudioWait";
                    break;
                }
            case 0x816:
                {
                    opcode.Name = "AudioWaitNotSkip";
                    break;
                }
            case 0x817:
                {
                    opcode.Name = "AudioWaitInput";
                    break;
                }
            case 0x818:
                {
                    opcode.Name = "AudioWaitInputNotSkip";
                    break;
                }
            case 0x819:
                {
                    opcode.Name = "AudioVolumeMain";
                    break;
                }
            case 0x81A:
                {
                    opcode.Name = "KoeRepeat";
                    break;
                }
            case 0x81B:
                {
                    opcode.Name = "CheckAudioPlay";
                    break;
                }
            case 0x81C:
                {
                    opcode.Name = "VoiceLinkSet";
                    break;
                }
            case 0x81D:
                {
                    opcode.Name = "VoiceLinkRelease";
                    break;
                }
            case 0x81E:
                {
                    opcode.Name = "VoiceLinkSetVolume";
                    break;
                }
            case 0x81F:
                {
                    opcode.Name = "KoePlayTest";
                    break;
                }
            case 0x820:
                {
                    opcode.Name = "KoeStopTest";
                    break;
                }
            
            //Video
            case 0x900:
                {
                    opcode.Name = "PlayVideo";
                    break;
                }
            case 0x901:
                {
                    opcode.Name = "PlayVideoPos";
                    break;
                }
            case 0x902:
                {
                    opcode.Name = "ReleaseVideo";
                    break;
                }
            case 0x903:
                {
                    opcode.Name = "VideoWait";
                    break;
                }
            case 0x904:
                {
                    opcode.Name = "VideoWaitNotSkip";
                    break;
                }
            case 0x905:
                {
                    opcode.Name = "SetVideoVolume";
                    break;
                }
            case 0x906:
                {
                    opcode.Name = "GetVideoPlayMode";
                    break;
                }
            
            case 0xA00:
                {
                    //Sets up the main HWND for the game, and sets the new window in case of the user wants to go from fullscreen to windowed.
                    
                    //While it is necessary to be called to configure the HWND itself, its only parameter is only used when the game is using
                    //their legacy window style (no DirectX support nor resizable screen) and has the exclusive fullscreen mode activated. The
                    //former is hardcoded onto the main game's executable, and the latter is a value taken directly from the system.dat file.
                    //So, based on how the developers set up both variables, the parameter for this opcode is largely ignored.
                    opcode.Name = "SetDisplay";

                    string screenMode = GetNumericParameter(inputFile, ref currentOffset);  //0: windowed mode
                                                                                            //Else: fullscreen mode
                    break;
                }
            case 0xA01:
                {
                    opcode.Name = "GameEnd";
                    break;
                }
            case 0xA02:
                {
                    opcode.Name = "SetGameEndCall";
                    break;
                }
            case 0xA03:
                {
                    opcode.Name = "SystemReset";
                    break;
                }
            case 0xA04:
                {
                    opcode.Name = "SystemResetEx";
                    break;
                }
            case 0xA05:
                {
                    opcode.Name = "SetSkip";
                    break;
                }
            case 0xA07:
                {
                    opcode.Name = "SetSkipMode";
                    break;
                }
            case 0xA08:
                {
                    opcode.Name = "SkipStart";
                    break;
                }
            case 0xA09:
                {
                    opcode.Name = "SkipStartSEL";
                    break;
                }
            case 0xA0A:
                {
                    opcode.Name = "SkipStop";
                    break;
                }
            case 0xA0B:
                {
                    opcode.Name = "SkipReleaseLock";
                    break;
                }
            case 0xA0C:
                {
                    opcode.Name = "ObjStock";
                    break;
                }
            case 0xA0D:
                {
                    opcode.Name = "SetColorBG";
                    break;
                }
            //Not understood
            case 0xA0E:
                {
                    //No VK code higher than 0x91 will be considered valid to be used. Also, if the second parameter is equal or smaller than 3, the opcode won't do anything.
                    opcode.Name = "SetKeyInput";

                    string virtualKeyCode = GetNumericParameter(inputFile, ref currentOffset);  //This indicates the Virtual-Key code that will activate the call to the label
                    string virtualKeyCode_Value = byte.TryParse(virtualKeyCode, out byte vkByte)
                        ? GetVirtualKeyCode_Constant(vkByte)            // The value is a literal, parse it directly
                        : virtualKeyCode;                              // The value is an indirect reference (DI[...]/DG[...]/DT[...])

                    string unknownParameter = GetNumericParameter(inputFile, ref currentOffset);  //This parameter is not understood, but it must higher than 3.

                    opcode.Parameters.AddRange(virtualKeyCode_Value, unknownParameter);
                    break;
                }
            case 0xA0F:
                {
                    //Makes that a specific key press (set through a Virtual-Key code (https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes))
                    //call the instructions tied to a specific label. An example is how the ESC button makes the game load the label to close the game. No VK code
                    //higher than 0x91 will be considered valid to be used.
                    opcode.Name = "SetKeyInputCallLabel";

                    string virtualKeyCode = GetNumericParameter(inputFile, ref currentOffset);  //This indicates the Virtual-Key code that will activate the call to the label
                    string virtualKeyCode_Value = byte.TryParse(virtualKeyCode, out byte vkByte)
                        ? GetVirtualKeyCode_Constant(vkByte)            // The value is a literal, parse it directly
                        : virtualKeyCode;                              // The value is an indirect reference (DI[...]/DG[...]/DT[...])
                    

                    string destination_SeenNumber = GetNumericParameter(inputFile, ref currentOffset);  //This indicates the Seen number that constains the label to jump to

                    string destination_LabelName = GetStringParameter(inputFile, ref currentOffset, encoding);    //This indicates the name of the label to jump to

                    opcode.Parameters.AddRange(virtualKeyCode_Value, destination_SeenNumber, destination_LabelName);
                    break;
                }
            case 0xA10:
                {
                    opcode.Name = "SetKeyInputCall";
                    break;
                }
            case 0xA11:
                {
                    opcode.Name = "SetKeyInputJumpLabel";
                    break;
                }
            case 0xA12:
                {
                    opcode.Name = "SetKeyInputJump";
                    break;
                }
            case 0xA13:
                {
                    opcode.Name = "InsKeySelect";
                    break;
                }
            case 0xA14:
                {
                    opcode.Name = "KeyInputPush";
                    break;
                }
            case 0xA15:
                {
                    opcode.Name = "KeyInputPop";
                    break;
                }
            case 0xA16:
                {
                    opcode.Name = "ReleaseKeyInputStack";
                    break;
                }
            case 0xA17:
                {
                    opcode.Name = "SubProcLabel";
                    break;
                }
            case 0xA18:
                {
                    opcode.Name = "SubProc";
                    break;
                }
            case 0xA19:
                {
                    opcode.Name = "ReleaseProc";
                    break;
                }
            case 0xA1A:
                {
                    opcode.Name = "ReleaseProcToNo";
                    break;
                }
            case 0xA1B:
                {
                    opcode.Name = "GetProcOrgData";
                    break;
                }
            case 0xA1C:
                {
                    opcode.Name = "LockProc";
                    break;
                }
            //Not understood
            case 0xA1D:
                {
                    //Not understood, it increases a global variable
                    opcode.Name = "ProcPriorityON";
                    break;
                }
            //Not understood
            case 0xA1E:
                {
                    opcode.Name = "ProcPriorityOFF";
                    break;
                }
            case 0xA1F:
                {
                    opcode.Name = "SetButtonOrgData";
                    break;
                }
            case 0xA20:
                {
                    opcode.Name = "SetButtonRect";
                    break;
                }
            case 0xA21:
                {
                    opcode.Name = "SetButtonOBJ";
                    break;
                }
            case 0xA22:
                {
                    opcode.Name = "SetButtonChain";
                    break;
                }
            case 0xA23:
                {
                    opcode.Name = "ReleaseButtonChain";
                    break;
                }
            case 0xA24:
                {
                    opcode.Name = "SelectButton";
                    break;
                }
            case 0xA25:
                {
                    opcode.Name = "ReleaseButton";
                    break;
                }
            case 0xA26:
                {
                    opcode.Name = "ReleaseButtonAll";
                    break;
                }
            case 0xA27:
                {
                    opcode.Name = "StopButton";
                    break;
                }
            case 0xA28:
                {
                    opcode.Name = "ButtonPush";
                    break;
                }
            case 0xA29:
                {
                    opcode.Name = "ButtonPop";
                    break;
                }
            case 0xA2A:
                {
                    opcode.Name = "ReleaseButtonStack";
                    break;
                }
            case 0xA2B:
                {
                    opcode.Name = "SetTextBox";
                    break;
                }
            case 0xA2C:
                {
                    opcode.Name = "SetTextBoxEx";
                    break;
                }
            case 0xA2D:
                {
                    opcode.Name = "ReleaseTextBox";
                    break;
                }
            case 0xA2E:
                {
                    opcode.Name = "SetFontBox";
                    break;
                }
            case 0xA2F:
                {
                    opcode.Name = "ReleaseFontBox";
                    break;
                }
            case 0xA30:
                {
                    opcode.Name = "MouseOnOff";
                    break;
                }
            case 0xA31:
                {
                    opcode.Name = "MousePos";
                    break;
                }
            case 0xA32:
                {
                    opcode.Name = "SetMouseCursor";
                    break;
                }
            case 0xA33:
                {
                    opcode.Name = "StartAutoMode";
                    break;
                }
            case 0xA34:
                {
                    opcode.Name = "StopAutoMode";
                    break;
                }
            case 0xA35:
                {
                    opcode.Name = "GetKey";
                    break;
                }
            case 0xA36:
                {
                    opcode.Name = "GetMouseWheel";
                    break;
                }
            case 0xA37:
                {
                    opcode.Name = "GetMouseWheelEx";
                    break;
                }
            case 0xA38:
                {
                    opcode.Name = "ResetWheelCount";
                    break;
                }
            case 0xA39:
                {
                    opcode.Name = "GetMouseInputDT";
                    break;
                }
            case 0xA3A:
                {
                    opcode.Name = "GetMouseDT";
                    break;
                }
            case 0xA3B:
                {
                    opcode.Name = "GetMousePushDT";
                    break;
                }
            case 0xA3C:
                {
                    opcode.Name = "ClearMousePushDT";
                    break;
                }
            case 0xA3D:
                {
                    //A simple if statement like in any normal language.
                    opcode.Name = "if";    //if{ according to the debug logs
                    
                    opcode.Parameters.Add(EvaluateCondition(inputFile, ref currentOffset)); //Get the entire if condition
                    opcode.Parameters.Add(
                        BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4)).ToString());    //Get the relative offset from the beginning of the opcode region to where this if ends (it should land where the opcode "}" is)
                    currentOffset += 4;
                    break;
                }
            case 0xA3E:
                {
                    //A simple else statement like in any normal language.
                    opcode.Name = "else";  //else{ according to the debug logs

                    opcode.Parameters.Add(
                        BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4)).ToString());    //Get the relative offset from the beginning of the opcode region to where this if ends (it should land where the opcode "}" is)
                    currentOffset += 4;

                    break;
                }
            case 0xA3F:
                {
                    opcode.Name = "while{";
                    break;
                }
            case 0xA40:
                {
                    opcode.Name = "break";
                    break;
                }
            case 0xA41:
                {
                    opcode.Name = "continue";
                    break;
                }
            case 0xA42:
                {
                    //Simple GOTO statement, jumps to an arbitrary piece of code.
                    opcode.Name = "GOTO";

                    opcode.Parameters.Add(
                        BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4)).ToString());    //Offset relative to the start of the opcode region, indicating how many bytes to jump from the start of said region.
                    currentOffset += 4;
                    break;
                }
            case 0xA43:
                {
                    opcode.Name = "Call";
                    break;
                }
            case 0xA44:
                {
                    //Jumps to a specific label from another Seen file
                    opcode.Name = "FarCallLabel";

                    opcode.Parameters.Add(GetNumericParameter(inputFile, ref currentOffset));   //Indicates the Seen nummber to jump to
                    opcode.Parameters.Add(GetStringParameter(inputFile, ref currentOffset, encoding));   //Indicates the name of the label to jump to
                    break;
                }
            case 0xA45:
                {
                    opcode.Name = "FarCall";
                    break;
                }
            case 0xA46:
                {
                    opcode.Name = "ReleaseCallStack";
                    break;
                }
            case 0xA47:
                {
                    opcode.Name = "return";
                    break;
                }
            case 0xA48:
                {
                    //Jumps to the beginning of a specific Seen file
                    opcode.Name = "Jump";

                    opcode.Parameters.Add(GetNumericParameter(inputFile, ref currentOffset)); //Indicates the Seen nummber to jump to
                    break;
                }
            case 0xA49:
                {
                    opcode.Name = "JumpLabel";
                    break;
                }
            case 0xA4A:
                {
                    opcode.Name = "Wait";
                    break;
                }
            case 0xA4B:
                {
                    opcode.Name = "WaitNotSkip";
                    break;
                }
            case 0xA4C:
                {
                    opcode.Name = "WaitInput";
                    break;
                }
            case 0xA4D:
                {
                    opcode.Name = "WaitInputNotSkip";
                    break;
                }
            case 0xA4E:
                {
                    opcode.Name = "StopInput";
                    break;
                }
            case 0xA4F:
                {
                    opcode.Name = "StopInputNotSkip";
                    break;
                }
            case 0xA50:
                {
                    opcode.Name = "WaitEffect";
                    break;
                }
            case 0xA51:
                {
                    opcode.Name = "Loop";
                    break;
                }
            case 0xA52:
                {
                    opcode.Name = "CountStart";
                    break;
                }
            case 0xA53:
                {
                    opcode.Name = "CountStop";
                    break;
                }
            case 0xA54:
                {
                    opcode.Name = "Count";
                    break;
                }
            case 0xA55:
                {
                    opcode.Name = "CountNotSkip";
                    break;
                }
            case 0xA56:
                {
                    opcode.Name = "CountInput";
                    break;
                }
            case 0xA57:
                {
                    opcode.Name = "CountInputNotSkip";
                    break;
                }
            case 0xA58:
                {
                    opcode.Name = "GetCountTime";
                    break;
                }
            case 0xA59:
                {
                    opcode.Name = "CountStart";
                    break;
                }
            case 0xA5A:
                {
                    opcode.Name = "strcmp";
                    break;
                }
            case 0xA5B:
                {
                    opcode.Name = "SetDispFPS";
                    break;
                }
            case 0xA5C:
                {
                    opcode.Name = "CountPouse";
                    break;
                }
            case 0xA5D:
                {
                    opcode.Name = "CountPouseEx";
                    break;
                }
            case 0xA5E:
                {
                    opcode.Name = "CountXspeed";
                    break;
                }
            case 0xA5F:
                {
                    opcode.Name = "CountXspeedEx";
                    break;
                }
            case 0xA60:
                {
                    opcode.Name = "GetCountTimeEx";
                    break;
                }
            case 0xA61:
                {
                    opcode.Name = "SetCountTimeEx";
                    break;
                }
            case 0xA62:
                {
                    opcode.Name = "AllCountStart";
                    break;
                }
            case 0xA63:
                {
                    opcode.Name = "AllCountXspeed";
                    break;
                }
            case 0xA64:
                {
                    opcode.Name = "AllCountStop";
                    break;
                }
            case 0xA65:
                {
                    opcode.Name = "AllCountPouse";
                    break;
                }
            case 0xA66:
                {
                    opcode.Name = "GetAllCountTime";
                    break;
                }
            case 0xA67:
                {
                    opcode.Name = "SetAllCountTime";
                    break;
                }
            case 0xA68:
                {
                    opcode.Name = "MouseHide";
                    break;
                }
            case 0xA69:
                {
                    opcode.Name = "SetButtonUpDown";
                    break;
                }
            case 0xA6A:
                {
                    opcode.Name = "MouseCheckBarrage";
                    break;
                }
            case 0xA6B:
                {
                    opcode.Name = "MouseCheckBarrageReset";
                    break;
                }
            case 0xA6C:
                {
                    opcode.Name = "MouseCheckDistance";
                    break;
                }
            case 0xA6D:
                {
                    opcode.Name = "MouseCapture";
                    break;
                }
            case 0xA6E:
                {
                    opcode.Name = "MouseCheckMovePoint";
                    break;
                }
            //Not understood
            case 0xA6F:
                {
                    //Increases a global variable, not well understood
                    opcode.Name = "MacroPriority_Set";
                    break;
                }
            //Not understood
            case 0xA70:
                {
                    //Decrease a global variable, not well understood
                    opcode.Name = "MacroPriority";
                    break;
                }
            case 0xA71:
                {
                    opcode.Name = "ReleaseKeyInputall";
                    break;
                }
            case 0xA72:
                {
                    opcode.Name = "MouseCheckClickCount";
                    break;
                }
            case 0xA73:
                {
                    opcode.Name = "MouseCheckClickCountReset";
                    break;
                }
            case 0xA74:
                {
                    opcode.Name = "AutoModeVoiceWait";
                    break;
                }
            case 0xA75:
                {
                    opcode.Name = "InProcCallLabel";
                    break;
                }
            case 0xA76:
                {
                    opcode.Name = "InProcCall";
                    break;
                }
            case 0xA77:
                {
                    opcode.Name = "InProcJumpLabel";
                    break;
                }
            case 0xA78:
                {
                    opcode.Name = "SetKeyInputJump";
                    break;
                }
            case 0xA79:
                {
                    opcode.Name = "ButtonInputOneLock";
                    break;
                }
            case 0xA7A:
                {
                    opcode.Name = "CheckSkip";
                    break;
                }
            case 0xA7B:
                {
                    opcode.Name = "GetScriptNo";
                    break;
                }
            case 0xA7C:
                {
                    opcode.Name = "D_WaitEffectInput";
                    break;
                }
            case 0xA7D:
                {
                    opcode.Name = "D_WaitEffectInputNotSkip";
                    break;
                }
            case 0xA7E:
                {
                    opcode.Name = "SetTextAllShow";
                    break;
                }
            case 0xA7F:
                {
                    opcode.Name = "LoadInsCallLock";
                    break;
                }
            case 0xA80:
                {
                    opcode.Name = "LoadToReturn";
                    break;
                }
            case 0xA81:
                {
                    opcode.Name = "GetGameEndMode";
                    break;
                }
            case 0xA82:
                {
                    opcode.Name = "ClearKeyPushDT";
                    break;
                }
            case 0xA83:
                {
                    opcode.Name = "StopButtonNoReset";
                    break;
                }
            case 0xA84:
                {
                    opcode.Name = "ReleaseButtonNoReset";
                    break;
                }
            case 0xA85:
                {
                    opcode.Name = "ReleaseButtonAllNoReset";
                    break;
                }
            case 0xA86:
                {
                    opcode.Name = "ChengeSelectCG";
                    break;
                }
            case 0xA87:
                {
                    opcode.Name = "ReLoadChengeCG";
                    break;
                }
            case 0xA88:
                {
                    opcode.Name = "GetChengeSelectCGNo";
                    break;
                }
            
            //Flags
            case 0xB00:
                {
                    opcode.Name = "InitDI";
                    break;
                }
            case 0xB01:
                {
                    opcode.Name = "InitDG";
                    break;
                }
            case 0xB02:
                {
                    opcode.Name = "InitDT";
                    break;
                }
            case 0xB03:
                {
                    opcode.Name = "InitSI";
                    break;
                }
            case 0xB04:
                {
                    opcode.Name = "InitSG";
                    break;
                }
            case 0xB05:
                {
                    opcode.Name = "InitST";
                    break;
                }
            case 0xB06:
                {
                    opcode.Name = "HenSetRange";
                    break;
                }
            case 0xB07:
                {
                    //Sets the value of a given DI, DG or DT flag
                    opcode.Name = "[] = Num";

                    opcode.Parameters.Add(GetFlagSlot_DX(inputFile, ref currentOffset)); //Indicates the slot in the corresponding array of flags that will store the given value
                    opcode.Parameters.Add(GetNumericParameter(inputFile, ref currentOffset));   //Indicates the value to store in the given flag
                    break;
                }
            case 0xB08:
                {
                    opcode.Name = "Num + Num";
                    break;
                }
            case 0xB09:
                {
                    opcode.Name = "Num - Num";
                    break;
                }
            case 0xB0A:
                {
                    opcode.Name = "Num * Num";
                    break;
                }
            case 0xB0B:
                {
                    opcode.Name = "Num / Num";
                    break;
                }
            case 0xB0C:
                {
                    opcode.Name = "Num % Num";
                    break;
                }
            case 0xB0D:
                {
                    opcode.Name = "Num & Num";
                    break;
                }
            case 0xB0E:
                {
                    opcode.Name = "Num | Num";
                    break;
                }
            case 0xB0F:
                {
                    opcode.Name = "Numm ^ Num";
                    break;
                }
            case 0xB10:
                {
                    opcode.Name = "[] = [+-*/%&|^] Num";
                    break;
                }
            case 0xB11:
                {
                    opcode.Name = "RAND";
                    break;
                }
            case 0xB12:
                {
                    opcode.Name = "CircleX";
                    break;
                }
            case 0xB13:
                {
                    opcode.Name = "CircleY";
                    break;
                }
            case 0xB14:
                {
                    opcode.Name = "Angle";
                    break;
                }
            case 0xB15:
                {
                    opcode.Name = "mdpL";
                    break;
                }
            case 0xB16:
                {
                    opcode.Name = "mdpR";
                    break;
                }
            case 0xB17:
                {
                    //Save a string to a specific flag
                    opcode.Name = "SetStr";

                    opcode.Parameters.Add(GetFlagSlot_SX(inputFile, ref currentOffset));     //Indicates the slot of the corresponding array of flags that will store the given value
                    opcode.Parameters.Add(GetStringParameter(inputFile, ref currentOffset, encoding));  //Indicates the value to store in the given flag
                    break;
                }
            case 0xB18:
                {
                    opcode.Name = "SetStrCat";
                    break;
                }
            case 0xB19:
                {
                    opcode.Name = "SetStrNum";
                    break;
                }
            case 0xB1A:
                {
                    opcode.Name = "SetStrNum2";
                    break;
                }
            case 0xB1B:
                {
                    opcode.Name = "StrPrintf";
                    break;
                }
            case 0xB1C:
                {
                    opcode.Name = "StrPrintf2";
                    break;
                }
            
            //OBJ and CGB
            case 0xD00:
                {
                    opcode.Name = "Disp";
                    break;
                }
            //Not understood
            case 0xD01:
                {
                    opcode.Name = "DispWait";
                    break;
                }
            case 0xD02:
                {
                    opcode.Name = "ShowOBJ";
                    break;
                }
            case 0xD03:
                {
                    opcode.Name = "LoadCGB";
                    break;
                }
            case 0xD04:
                {
                    opcode.Name = "OBJCopyToCGB";
                    break;
                }
            case 0xD05:
                {
                    opcode.Name = "CGBCopyToCGB";
                    break;
                }
            case 0xD06:
                {
                    opcode.Name = "ReleaseCGB";
                    break;
                }
            case 0xD07:
                {
                    opcode.Name = "ReleaseCGBRange";
                    break;
                }
            //Not well understood
            case 0xD08:
                {
                    //Loads a given file (from data00)
                    opcode.Name = "LoadOBJ";

                    opcode.Parameters.Add(GetNumericParameter(inputFile, ref currentOffset));   //Indicates the slot from a particular type of flag to get the OBJ from (not quite understood)
                    opcode.Parameters.Add(GetStringParameter(inputFile, ref currentOffset, encoding));     //Indicates the flag number where the name of the file to load is
                    break;
                }
            case 0xD09:
                {
                    opcode.Name = "CGBLink";
                    break;
                }
            case 0xD0A:
                {
                    opcode.Name = "CGBCopy";
                    break;
                }
            case 0xD0B:
                {
                    opcode.Name = "ReleaseOBJ";
                    break;
                }
            case 0xD0C:
                {
                    opcode.Name = "SetAniPat";
                    break;
                }
            case 0xD0D:
                {
                    opcode.Name = "SetAniPatEx";
                    break;
                }
            case 0xD0E:
                {
                    opcode.Name = "ReleaseAniPat";
                    break;
                }
            case 0xD0F:
                {
                    opcode.Name = "InitAni";
                    break;
                }
            case 0xD10:
                {
                    opcode.Name = "SetAniPlay";
                    break;
                }
            case 0xD11:
                {
                    opcode.Name = "AniMovePlay";
                    break;
                }
            case 0xD12:
                {
                    opcode.Name = "AniMoveStop";
                    break;
                }
            case 0xD13:
                {
                    opcode.Name = "AniMoveWait";
                    break;
                }
            case 0xD14:
                {
                    opcode.Name = "AniMoveWaitNotSkip";
                    break;
                }
            case 0xD15:
                {
                    opcode.Name = "AniMoveWaitInput";
                    break;
                }
            case 0xD16:
                {
                    opcode.Name = "AniMoveWaitInputNotSkip";
                    break;
                }
            case 0xD17:
                {
                    opcode.Name = "TextOBJ";
                    break;
                }
            case 0xD18:
                {
                    opcode.Name = "TextCGB";
                    break;
                }
            case 0xD19:
                {
                    opcode.Name = "TextOBJEx";
                    break;
                }
            case 0xD1A:
                {
                    opcode.Name = "TextCGBEx";
                    break;
                }
            case 0xD1B:
                {
                    opcode.Name = "NumOBJ";
                    break;
                }
            case 0xD1C:
                {
                    opcode.Name = "NumCGB";
                    break;
                }
            //UNKNOWN HOW IT WORKS
            case 0xD1D:
                {
                    opcode.Name = "ObjPosDest";

                    // string[] value = GetNumericParameter(inputFile, ref currentOffset);
                    // string value_Source = value[0];    //Indicates that the numeric value is obtained from the following parameter
                    // string value_Number = value[1];    //Indicates the slot from the particular type of flag to get OBJ from

                    // opcode.Parameters.Add(value_Source);
                    // opcode.Parameters.Add(value_Number);

                    // string[] value = GetNumericParameter(inputFile, ref currentOffset);
                    // string value_Source = value[0];    //Indicates that the numeric value is obtained from the following parameter
                    // string value_Number = value[1];    //Indicates the slot from the particular type of flag to get OBJ from

                    // opcode.Parameters.Add(value_Source);
                    // opcode.Parameters.Add(value_Number);

                    // string[] value = GetNumericParameter(inputFile, ref currentOffset);
                    // string value_Source = value[0];    //Indicates that the numeric value is obtained from the following parameter
                    // string value_Number = value[1];    //Indicates the slot from the particular type of flag to get OBJ from

                    // opcode.Parameters.Add(value_Source);
                    // opcode.Parameters.Add(value_Number);

                    break;
                }
            case 0xD1E:
                {
                    opcode.Name = "ObjPosDestOffset";
                    break;
                }
            case 0xD1F:
                {
                    opcode.Name = "ObjPosScr";
                    break;
                }
            case 0xD20:
                {
                    opcode.Name = "ObjPosScrOffset";
                    break;
                }
            case 0xD21:
                {
                    opcode.Name = "ObjSizDest";
                    break;
                }
            case 0xD22:
                {
                    opcode.Name = "ObjSizDestOffset";
                    break;
                }
            case 0xD23:
                {
                    opcode.Name = "ObjSizScr";
                    break;
                }
            case 0xD24:
                {
                    opcode.Name = "ObjSizScrOffset";
                    break;
                }
            case 0xD25:
                {
                    opcode.Name = "ObjSiz";
                    break;
                }
            case 0xD26:
                {
                    opcode.Name = "ObjSizOffset";
                    break;
                }
            case 0xD27:
                {
                    opcode.Name = "ObjRectDest";
                    break;
                }
            case 0xD28:
                {
                    opcode.Name = "ObjRectDestOffset";
                    break;
                }
            case 0xD29:
                {
                    opcode.Name = "ObjRectScr";
                    break;
                }
            case 0xD2A:
                {
                    opcode.Name = "ObjRectScrOffset";
                    break;
                }
            case 0xD2B:
                {
                    opcode.Name = "ObjRect";
                    break;
                }
            case 0xD2C:
                {
                    opcode.Name = "ObjRectOffset";
                    break;
                }
            case 0xD2D:
                {
                    opcode.Name = "ObjZoomMode";
                    break;
                }
            case 0xD2E:
                {
                    opcode.Name = "ObjAlpha";
                    break;
                }
            case 0xD2F:
                {
                    opcode.Name = "ObjColor";
                    break;
                }
            case 0xD30:
                {
                    opcode.Name = "ObjValue";
                    break;
                }
            case 0xD31:
                {
                    opcode.Name = "ObjLight";
                    break;
                }
            case 0xD32:
                {
                    opcode.Name = "ObjSai";
                    break;
                }
            case 0xD33:
                {
                    opcode.Name = "CopyOBJ";
                    break;
                }
            case 0xD34:
                {
                    opcode.Name = "MoveOBJ";
                    break;
                }
            case 0xD35:
                {
                    opcode.Name = "ChengeOBJ";
                    break;
                }
            case 0xD36:
                {
                    opcode.Name = "BakToOBJ";
                    break;
                }
            case 0xD37:
                {
                    opcode.Name = "AllShowOBJ";
                    break;
                }
            case 0xD38:
                {
                    opcode.Name = "AllObjPosDestOffset";
                    break;
                }
            case 0xD39:
                {
                    opcode.Name = "AllObjPosScrOffset";
                    break;
                }
            case 0xD3A:
                {
                    opcode.Name = "AllObjSizDestOffset";
                    break;
                }
            case 0xD3B:
                {
                    opcode.Name = "AllObjSizScrOffset";
                    break;
                }
            case 0xD3C:
                {
                    opcode.Name = "AllObjSizOffset";
                    break;
                }
            case 0xD3D:
                {
                    opcode.Name = "AllObjRectDestOffset";
                    break;
                }
            case 0xD3E:
                {
                    opcode.Name = "AllObjRectScrOffset";
                    break;
                }
            case 0xD3F:
                {
                    opcode.Name = "AllObjRectOffset";
                    break;
                }
            case 0xD40:
                {
                    opcode.Name = "AllObjAlpha";
                    break;
                }
            case 0xD41:
                {
                    opcode.Name = "AllObjColor";
                    break;
                }
            case 0xD42:
                {
                    opcode.Name = "AllObjValue";
                    break;
                }
            case 0xD43:
                {
                    opcode.Name = "AllObjLight";
                    break;
                }
            case 0xD44:
                {
                    opcode.Name = "AllObjSai";
                    break;
                }
            case 0xD45:
                {
                    opcode.Name = "AllBakToOBJ";
                    break;
                }
            case 0xD46:
                {
                    opcode.Name = "ObjSpin";
                    break;
                }
            case 0xD47:
                {
                    opcode.Name = "AniMoveSpeedPuls";
                    break;
                }
            case 0xD48:
                {
                    opcode.Name = "AniMoveSpeedXX";
                    break;
                }
            case 0xD49:
                {
                    opcode.Name = "LoadOBJAniFrame";
                    break;
                }
            case 0xD4A:
                {
                    opcode.Name = "LoadOBJTrans";
                    break;
                }
            case 0xD4B:
                {
                    opcode.Name = "CGBCopyTrans";
                    break;
                }
            case 0xD4C:
                {
                    opcode.Name = "OBJCopyToCGB";
                    break;
                }
            case 0xD4D:
                {
                    opcode.Name = "CGBCopyToCGB";
                    break;
                }
            case 0xD4E:
                {
                    opcode.Name = "OBJTransCopy";
                    break;
                }
            case 0xD4F:
                {
                    opcode.Name = "LoadOBJAniFrameTrans";
                    break;
                }
            case 0xD50:
                {
                    opcode.Name = "LoadOBJAniFrameTransEx";
                    break;
                }
            case 0xD51:
                {
                    opcode.Name = "GetObjPos";
                    break;
                }
            case 0xD52:
                {
                    opcode.Name = "AniMoveLoop";
                    break;
                }
            case 0xD53:
                {
                    opcode.Name = "ObjReColor";
                    break;
                }
            case 0xD54:
                {
                    opcode.Name = "AllReleaseOBJ";
                    break;
                }
            case 0xD55:
                {
                    opcode.Name = "AniMoveTrans";
                    break;
                }
            case 0xD56:
                {
                    opcode.Name = "AniMovePlayTrans";
                    break;
                }
            case 0xD57:
                {
                    opcode.Name = "DISPLock";
                    break;
                }
            case 0xD58:
                {
                    opcode.Name = "ObjTrans";
                    break;
                }
            case 0xD59:
                {
                    opcode.Name = "GetObjFileName";
                    break;
                }
            case 0xD5A:
                {
                    opcode.Name = "GetShowOBJ";
                    break;
                }
            case 0xD5B:
                {
                    opcode.Name = "GetObjSize";
                    break;
                }
            case 0xD5C:
                {
                    opcode.Name = "ObjTransReSize";
                    break;
                }
            case 0xD5D:
                {
                    opcode.Name = "AniMovePlayCheck";
                    break;
                }
            case 0xD5E:
                {
                    opcode.Name = "GetObjFileNameEx";
                    break;
                }
            case 0xD5F:
                {
                    opcode.Name = "AniMoveSkipONOFF";
                    break;
                }
            case 0xD60:
                {
                    opcode.Name = "GetObjRect";
                    break;
                }
            case 0xD61:
                {
                    opcode.Name = "AllHideReleaseOBJ";
                    break;
                }
            case 0xD62:
                {
                    opcode.Name = "D_AniMovePlayControl";
                    break;
                }
            case 0xD63:
                {
                    opcode.Name = "AniMoveListPlay";
                    break;
                }
            case 0xD64:
                {
                    opcode.Name = "AniMoveListPlayTrans";
                    break;
                }
            
            //Message window
            case 0xE00:
                {
                    opcode.Name = "MSGWNDNAME";
                    break;
                }
            case 0xE01:
                {
                    opcode.Name = "MSGWNDTEXT";
                    break;
                }
            case 0xE02:
                {
                    opcode.Name = "SEL";
                    break;
                }
            case 0xE03:
                {
                    opcode.Name = "SetInsStrNo";
                    break;
                }
            case 0xE04:
                {
                    opcode.Name = "R";
                    break;
                }
            case 0xE05:
                {
                    opcode.Name = "D";
                    break;
                }
            case 0xE06:
                {
                    opcode.Name = "PAUSE";
                    break;
                }
            case 0xE07:
                {
                    opcode.Name = "SetMsgWndMode";
                    break;
                }
            case 0xE08:
                {
                    opcode.Name = "MsgWndShow";
                    break;
                }
            case 0xE09:
                {
                    opcode.Name = "MsgWndObjShow";
                    break;
                }
            case 0xE0A:
                {
                    opcode.Name = "SelectMsgWnd";
                    break;
                }
            case 0xE0B:
                {
                    opcode.Name = "SelectSelWnd";
                    break;
                }
            case 0xE0C:
                {
                    opcode.Name = "SetFontNo";
                    break;
                }
            case 0xE0D:
                {
                    opcode.Name = "SetFontTypeFace";
                    break;
                }
            case 0xE0E:
                {
                    opcode.Name = "SetKaigyouToFont";
                    break;
                }
            case 0xE0F:
                {
                    opcode.Name = "SetKaigyouToPixel";
                    break;
                }
            case 0xE10:
                {
                    opcode.Name = "SetTextSpeed";
                    break;
                }
            case 0xE11:
                {
                    opcode.Name = "TxtDefaultColor";
                    break;
                }
            case 0xE12:
                {
                    opcode.Name = "TxtColorLock";
                    break;
                }
            case 0xE13:
                {
                    opcode.Name = "TxtColorON";
                    break;
                }
            case 0xE14:
                {
                    opcode.Name = "TxtColorOFF";
                    break;
                }
            case 0xE15:
                {
                    opcode.Name = "MsgWndBgColor";
                    break;
                }
            case 0xE16:
                {
                    opcode.Name = "MsgTxtClear";
                    break;
                }
            case 0xE17:
                {
                    opcode.Name = "SetTxtChengeFade";
                    break;
                }
            case 0xE18:
                {
                    opcode.Name = "SetTxtPutFade";
                    break;
                }
            case 0xE19:
                {
                    opcode.Name = "SetClickOBJ";
                    break;
                }
            case 0xE1A:
                {
                    opcode.Name = "TxtShadow";
                    break;
                }
            case 0xE1B:
                {
                    opcode.Name = "SelEXflgparm";
                    break;
                }
            case 0xE1C:
                {
                    opcode.Name = "SelEXSet";
                    break;
                }
            case 0xE1D:
                {
                    opcode.Name = "SelEXLoop";
                    break;
                }
            case 0xE1E:
                {
                    opcode.Name = "SelEXEnd";
                    break;
                }
            case 0xE1F:
                {
                    opcode.Name = "D_SelEXShow";
                    break;
                }
            case 0xE20:
                {
                    opcode.Name = "GetSelCount";
                    break;
                }
            case 0xE21:
                {
                    opcode.Name = "GetSelObjNo";
                    break;
                }
            case 0xE22:
                {
                    opcode.Name = "GetSelObjPos";
                    break;
                }
            case 0xE23:
                {
                    opcode.Name = "GetSelKidoku";
                    break;
                }
            case 0xE24:
                {
                    opcode.Name = "MsgWndFrameReColor";
                    break;
                }
            case 0xE25:
                {
                    opcode.Name = "TxtKidokuSpeed";
                    break;
                }
            case 0xE26:
                {
                    opcode.Name = "SetKoeRepeatOBJ";
                    break;
                }
            case 0xE27:
                {
                    opcode.Name = "CheckMsgWait";
                    break;
                }
            case 0xE28:
                {
                    opcode.Name = "Click";
                    break;
                }
            case 0xE29:
                {
                    opcode.Name = "CheckMsgWndShow";
                    break;
                }
            case 0xE2A:
                {
                    opcode.Name = "SelEXPulsSet";
                    break;
                }
            case 0xE2B:
                {
                    opcode.Name = "SelEXPulsReSet";
                    break;
                }
            case 0xE2C:
                {
                    opcode.Name = "NoWaitR";
                    break;
                }
            
            //Effects
            case 0xF00:
                {
                    opcode.Name = "EFObjAlpha";
                    break;
                }
            case 0xF01:
                {
                    opcode.Name = "EFAllAlpha";
                    break;
                }
            case 0xF02:
                {
                    opcode.Name = "EFAllBAlpha";
                    break;
                }
            case 0xF03:
                {
                    opcode.Name = "EFAllAlphaC";
                    break;
                }
            case 0xF04:
                {
                    opcode.Name = "EFAllBAlphaC";
                    break;
                }
            case 0xF05:
                {
                    opcode.Name = "EFAllColor";
                    break;
                }
            case 0xF06:
                {
                    opcode.Name = "EFAllBColor";
                    break;
                }
            case 0xF07:
                {
                    opcode.Name = "EFSObjAlpha";
                    break;
                }
            case 0xF08:
                {
                    opcode.Name = "EJSObjColor";
                    break;
                }
            case 0xF09:
                {
                    opcode.Name = "EFSObjRectDest";
                    break;
                }
            case 0xF0A:
                {
                    opcode.Name = "EFSObjRectScr";
                    break;
                }
            case 0xF0B:
                {
                    opcode.Name = "EFSObjRectDestOffset";
                    break;
                }
            case 0xF0C:
                {
                    opcode.Name = "EFSObjRectScrOffset";
                    break;
                }
            case 0xF0D:
                {
                    opcode.Name = "EFSObjPosDest";
                    break;
                }
            case 0xF0E:
                {
                    opcode.Name = "EFSObjPosScr";
                    break;
                }
            case 0xF0F:
                {
                    opcode.Name = "EFSObjPosDestOffset";
                    break;
                }
            case 0xF10:
                {
                    opcode.Name = "EFSObjPosScrOffset";
                    break;
                }
            case 0xF11:
                {
                    opcode.Name = "EFSObjSpin";
                    break;
                }
            case 0xF12:
                {
                    opcode.Name = "EFSObjPosSpinDest";
                    break;
                }
            case 0xF13:
                {
                    opcode.Name = "EFSObjPosSpinScr";
                    break;
                }
            case 0xF14:
                {
                    opcode.Name = "ObjShake";
                    break;
                }
            case 0xF15:
                {
                    opcode.Name = "ObjShakeEx";
                    break;
                }
            case 0xF16:
                {
                    opcode.Name = "EFASObjColor";
                    break;
                }
            case 0xF17:
                {
                    opcode.Name = "EFASObjRectDestOffset";
                    break;
                }
            case 0xF18:
                {
                    opcode.Name = "EFASObjRectScrOffset";
                    break;
                }
            case 0xF19:
                {
                    opcode.Name = "EFASObjPosDestOffset";
                    break;
                }
            case 0xF1A:
                {
                    opcode.Name = "EFASObjPosScrOffset";
                    break;
                }
            case 0xF1B:
                {
                    opcode.Name = "StopEffectLS";
                    break;
                }
            case 0xF1C:
                {
                    opcode.Name = "EffectEnd";
                    break;
                }
            case 0xF1D:
                {
                    opcode.Name = "LoadMSK";
                    break;
                }
            case 0xF1E:
                {
                    opcode.Name = "ReleaseMSK";
                    break;
                }
            case 0xF1F:
                {
                    opcode.Name = "EFObjAlphaMsk";
                    break;
                }
            case 0xF20:
                {
                    opcode.Name = "EFAllAlphaMsk";
                    break;
                }
            case 0xF21:
                {
                    opcode.Name = "EFAllAlphaMskB";
                    break;
                }
            case 0xF22:
                {
                    opcode.Name = "EFAllAlphaMskC";
                    break;
                }
            case 0xF23:
                {
                    opcode.Name = "EFAllAlphaMskCB";
                    break;
                }
            case 0xF24:
                {
                    opcode.Name = "EFObjAlphaMskMove";
                    break;
                }
            case 0xF25:
                {
                    opcode.Name = "EFObjMskMove";
                    break;
                }
            case 0xF26:
                {
                    opcode.Name = "EFAllAlphaMskMove";
                    break;
                }
            case 0xF27:
                {
                    opcode.Name = "EFAllAlphaMskMoveB";
                    break;
                }
            case 0xF28:
                {
                    opcode.Name = "EFAllMskMove";
                    break;
                }
            case 0xF29:
                {
                    opcode.Name = "EFAllMskMoveB";
                    break;
                }
            case 0xF2A:
                {
                    opcode.Name = "EFAllAlphaMskCMove";
                    break;
                }
            case 0xF2B:
                {
                    opcode.Name = "EFAllAlphaMskCMoveB";
                    break;
                }
            case 0xF2C:
                {
                    opcode.Name = "EFAllMskCMove";
                    break;
                }
            case 0xF2D:
                {
                    opcode.Name = "EFAllMskCMoveB";
                    break;
                }
            case 0xF2E:
                {
                    opcode.Name = "EFAllAlphaZoom";
                    break;
                }
            case 0xF2F:
                {
                    opcode.Name = "EFAllAlphaZoomB";
                    break;
                }
            case 0xF30:
                {
                    opcode.Name = "EFAllAlphaZoomBak";
                    break;
                }
            case 0xF31:
                {
                    opcode.Name = "EFAllAlphaZoomBakB";
                    break;
                }
            case 0xF32:
                {
                    opcode.Name = "EFAObjAlpha";
                    break;
                }
            case 0xF33:
                {
                    opcode.Name = "EFAObjAlphaMsk";
                    break;
                }
            case 0xF34:
                {
                    opcode.Name = "EFAObjAlphaMskMove";
                    break;
                }
            case 0xF35:
                {
                    opcode.Name = "EFAObjMskMove";
                    break;
                }
            case 0xF36:
                {
                    opcode.Name = "EFGObjAlpha";
                    break;
                }
            case 0xF37:
                {
                    opcode.Name = "EFGObjAlphaMsk";
                    break;
                }
            case 0xF38:
                {
                    opcode.Name = "EFGObjAlphaMskMove";
                    break;
                }
            case 0xF39:
                {
                    opcode.Name = "EFGObjMskMove";
                    break;
                }
            case 0xF3A:
                {
                    opcode.Name = "Shake";
                    break;
                }
            case 0xF3B:
                {
                    opcode.Name = "ShakeBC";
                    break;
                }
            case 0xF3C:
                {
                    opcode.Name = "ShakeEx";
                    break;
                }
            case 0xF3D:
                {
                    opcode.Name = "ShakeBCEx";
                    break;
                }
            case 0xF3E:
                {
                    opcode.Name = "ProgEFON";
                    break;
                }
            case 0xF3F:
                {
                    opcode.Name = "ProgEFGroupON";
                    break;
                }
            case 0xF40:
                {
                    opcode.Name = "ProgEFOFF";
                    break;
                }
            case 0xF41:
                {
                    opcode.Name = "ProgEFSetMsk";
                    break;
                }
            case 0xF42:
                {
                    opcode.Name = "ProgEFSetMskPos";
                    break;
                }
            case 0xF43:
                {
                    opcode.Name = "ProgEFSetAlpha";
                    break;
                }
            case 0xF44:
                {
                    opcode.Name = "ProgEFSetMskMove";
                    break;
                }
            case 0xF45:
                {
                    opcode.Name = "ProgEFSetAlphaFade";
                    break;
                }
            case 0xF46:
                {
                    opcode.Name = "ProgEFCount";
                    break;
                }
            case 0xF47:
                {
                    opcode.Name = "ProgEFWait";
                    break;
                }
            case 0xF48:
                {
                    opcode.Name = "AllEFMode";
                    break;
                }
            case 0xF49:
                {
                    opcode.Name = "IsEffect";
                    break;
                }
            case 0xF4A:
                {
                    opcode.Name = "IsEffectALL";
                    break;
                }
            case 0xF4B:
                {
                    opcode.Name = "EFSObjXspeed";
                    break;
                }
            case 0xF4C:
                {
                    opcode.Name = "EFSObjPause";
                    break;
                }
            case 0xF4D:
                {
                    opcode.Name = "EffectLS_AniMoveTrans";
                    break;
                }
            case 0xF4E:
                {
                    opcode.Name = "EffectLS_Trans";
                    break;
                }
            case 0xF4F:
                {
                    opcode.Name = "EffectLS_TransReSize";
                    break;
                }
            
            case 0xFFFE:
                {   
                    //"If", "else" and "while" terminator statement. If it's for a "while" statement, it loops back to the beginning of the loop.
                    opcode.Name = "}";
                    
                    ushort blockType = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2)); //Indicates if the block is an "if", "else" or "while"
                    currentOffset += 2;

                    if (blockType == 0xA3F) //"while" block
                    {
                        opcode.Parameters.Add("while");
                        opcode.Parameters.Add(
                            BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4)).ToString());    //Get the relative offset from the beginning of the opcode region to where this "while" loop starts
                    }
                    else if (blockType == 0xA3D)    //"if" block
                    {
                        opcode.Parameters.Add("if");
                        opcode.Parameters.Add(
                            BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4)).ToString());    //Get the current line (it's only used for debugging purposes in-game)
                    }
                    else    //"else" block
                    {
                        opcode.Parameters.Add("else");
                        opcode.Parameters.Add(
                            BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4)).ToString());    //Get the current line (it's only used for debugging purposes in-game)
                    }

                    currentOffset += 4;

                    break;
                }
            default:
                {
                    //The game features some opcodes like 0x3, that really don't do anything at all. In that case
                    //the engine doesn't do anything.
                    currentOffset += 2;
                    break;
                }
                
        }
    
        return opcode;
    }

    /// <summary>
    /// Parse the array of the compiled SDT, in order to obtain an arbitrary string parameter.
    /// </summary>
    /// <param name="inputFile">The compiled SDT file.</param>
    /// <param name="currentOffset">The offset where to start parsing values.</param>
    /// <returns>The value itself or the index in the corresponding array of flags.</returns>
    /// <exception cref="Exception">The type of value still has not been understood.</exception>
    /// <exception cref="InvalidDataException">The given type does not exist.</exception>
    private static string GetStringParameter(byte[] inputFile, ref int currentOffset, Encoding encoding)
    {
        string parameter;

        switch(inputFile[currentOffset++])
        {
            case 0x8:
                {
                    parameter = GetEncrytedString(inputFile, ref currentOffset, encoding);
                    break;
                }
            case 0x9:
                {
                    uint value = BinaryPrimitives.ReadUInt32LittleEndian(
                        inputFile.AsSpan(currentOffset, 4));
                    currentOffset += 4;

                    parameter = "SI[" + value + "]";
                    break;
                }
            case 0xA:
                {
                    uint value = BinaryPrimitives.ReadUInt32LittleEndian(
                        inputFile.AsSpan(currentOffset, 4));
                    currentOffset += 4;

                    parameter = "SG[" + value + "]";
                    break;
                }
            case 0xB:
                {
                    uint value = BinaryPrimitives.ReadUInt32LittleEndian(
                        inputFile.AsSpan(currentOffset, 4));
                    currentOffset += 4;

                    parameter = "ST[" + value + "]";
                    break;
                }
            case 0xD:
                {
                    throw new Exception("Unmanaged case in offset " + (currentOffset - 1));
                }
            case 0xE:
                {
                    throw new Exception("Unmanaged case in offset " + (currentOffset - 1));
                }
            case 0xF:
                {
                    throw new Exception("Unmanaged case in offset " + (currentOffset - 1));
                }
            default:
                    throw new InvalidDataException("There's an invalid type of opcode in offset " + (currentOffset - 1));
        }

        return parameter;
    }

    /// <summary>
    /// The game encrypts the strings with a pretty basic XOR, this decrypts the given string.
    /// </summary>
    /// <param name="inputFile">The compiled SDT file.</param>
    /// <param name="currentOffset">The offset where to start parsing values.</param>
    /// <param name="encoding">The encoding used for the string.</param>
    /// <returns>The decrypted string.</returns>
    private static string GetEncrytedString(byte[] inputFile, ref int currentOffset, Encoding encoding)
    {
        List<byte> labelName_Raw = [];
        while(inputFile[currentOffset] != 0xFF)
            labelName_Raw.Add((byte)(inputFile[currentOffset++] ^ 0xFF));

        currentOffset++; //Skip the 0xFF terminator byte

        return encoding.GetString(labelName_Raw.ToArray());
    }

    /// <summary>
    /// Parse the array of the compiled SDT, in order to obtain an arbitrary numeric value.
    /// </summary>
    /// <param name="inputFile">The compiled SDT file.</param>
    /// <param name="currentOffset">The offset where to start parsing values.</param>
    /// <returns>The value itself or the index in the corresponding array of flags.</returns>
    /// <exception cref="InvalidDataException">The given type does not exist.</exception>
    private static string GetNumericParameter(byte[] inputFile, ref int currentOffset)
    {
        string parameter;

        switch(inputFile[currentOffset++])
        {
            case 0x0:
                {
                    uint value = BinaryPrimitives.ReadUInt32LittleEndian(
                        inputFile.AsSpan(currentOffset, 4));
                    currentOffset += 4;

                    parameter = value.ToString();
                    break;
                }
            case 0x1:
                {
                    uint value = BinaryPrimitives.ReadUInt32LittleEndian(
                        inputFile.AsSpan(currentOffset, 4));
                    currentOffset += 4;

                    parameter = "DI[" + value + "]";
                    break;
                }
            case 0x2:
                {
                    uint value = BinaryPrimitives.ReadUInt32LittleEndian(
                        inputFile.AsSpan(currentOffset, 4));
                    currentOffset += 4;

                    parameter = "DG[" + value + "]";
                    break;
                }
            case 0x3:
                {
                    uint value = BinaryPrimitives.ReadUInt32LittleEndian(
                        inputFile.AsSpan(currentOffset, 4));
                    currentOffset += 4;

                    parameter = "DT[" + value + "]";
                    break;
                }
            case 0x5:
                {
                    string index = GetNumericParameter(inputFile, ref currentOffset);
                    parameter = "DI[" + index + "]";
                    break;
                }
            case 0x6:
                {
                    string index = GetNumericParameter(inputFile, ref currentOffset);
                    parameter = "DG[" + index + "]";
                    break;
                }
            case 0x7:
                {
                    string index = GetNumericParameter(inputFile, ref currentOffset);
                    parameter = "DT[" + index + "]";
                    break;
                }
            default:
                throw new InvalidDataException("Invalid type of opcode in offset " + (currentOffset - 1));
        }

        return parameter;
    }

    /// <summary>
    /// Parse the array of the compiled SDT, in order to obtain the type of flag and its
    /// respective slot. This is for the DI, DG and DT flags.
    /// </summary>
    /// <param name="inputFile">The compiled SDT file.</param>
    /// <param name="currentOffset">The offset where to start parsing values.</param>
    /// <returns>The index in the corresponding array of flags.</returns>
    /// <exception cref="Exception">The type of value still has not been understood.</exception>
    /// <exception cref="InvalidDataException">The given type does not exist.</exception>
    private static string GetFlagSlot_DX(byte[] inputFile, ref int currentOffset)
    {
        string parameter;

        switch(inputFile[currentOffset++])
        {
            case 0x1:
                {
                    ushort value = BinaryPrimitives.ReadUInt16LittleEndian(
                        inputFile.AsSpan(currentOffset, 2));    //For some reason, the game only reads this value as an ushort
                    currentOffset += 4;

                    parameter = "DI[" + value + "]";
                    break;
                }
            case 0x2:
                {
                    ushort value = BinaryPrimitives.ReadUInt16LittleEndian(
                        inputFile.AsSpan(currentOffset, 2));    //For some reason, the game only reads this value as an ushort
                    currentOffset += 4;

                    parameter = "DG[" + value + "]";
                    break;
                }
            case 0x3:
                {
                    ushort value = BinaryPrimitives.ReadUInt16LittleEndian(
                        inputFile.AsSpan(currentOffset, 2));    //For some reason, the game only reads this value as an ushort
                    currentOffset += 4;

                    parameter = "DT[" + value + "]";
                    break;
                }
            case 0x5:
                {
                    // parameter.Add("DI");
                    throw new Exception("Unmanaged case in offset " + (currentOffset - 1));
                }
            case 0x6:
                {
                    // parameters.Add("DG");
                    throw new Exception("Unmanaged case in offset " + (currentOffset - 1));
                }
            case 0x7:
                {
                    // parameters.Add("DT");
                    throw new Exception("Unmanaged case in offset " + (currentOffset - 1));
                }
            default:
                throw new InvalidDataException("Invalid type of opcode in offset " + (currentOffset - 1));
        }

        return parameter;
    }

    /// <summary>
    /// Parse the array of the compiled SDT, in order to obtain the type of flag and its
    /// respective slot. This is for the SI, SG and ST flags.
    /// </summary>
    /// <param name="inputFile">The compiled SDT file.</param>
    /// <param name="currentOffset">The offset where to start parsing values.</param>
    /// <returns>The index in the corresponding array of flags.</returns>
    /// <exception cref="Exception">The type of value still has not been understood.</exception>
    /// <exception cref="InvalidDataException">The given type does not exist.</exception>
    private static string GetFlagSlot_SX(byte[] inputFile, ref int currentOffset)
    {
        string parameter;

        switch(inputFile[currentOffset++])
        {
            case 0x9:
                {
                    ushort value = BinaryPrimitives.ReadUInt16LittleEndian(
                        inputFile.AsSpan(currentOffset, 2));    //For some reason, the game only reads this value as an ushort
                    currentOffset += 4;

                    parameter = "SI[" + value + "]";
                    break;
                }
            case 0xA:
                {
                    ushort value = BinaryPrimitives.ReadUInt16LittleEndian(
                        inputFile.AsSpan(currentOffset, 2));    //For some reason, the game only reads this value as an ushort
                    currentOffset += 4;

                    parameter = "SG[" + value + "]";
                    break;
                }
            case 0xB:
                {
                    ushort value = BinaryPrimitives.ReadUInt16LittleEndian(
                        inputFile.AsSpan(currentOffset, 2));    //For some reason, the game only reads this value as an ushort
                    currentOffset += 4;

                    parameter = "ST[" + value + "]";
                    break;
                }
            case 0xD:
                {
                    // parameters.Add("SI");
                    throw new Exception("Unmanaged case in offset " + (currentOffset - 1));
                }
            case 0xE:
                {
                    // parameters.Add("SG");
                    throw new Exception("Unmanaged case in offset " + (currentOffset - 1));
                }
            case 0xF:
                {
                    // parameters.Add("ST");
                    throw new Exception("Unmanaged case in offset " + (currentOffset - 1));
                }
            default:
                throw new InvalidDataException("Invalid type of opcode in offset " + (currentOffset - 1));
        }

        return parameter;
    }

    /// <summary>
    /// Function used to generate if and while conditions, allowing for nested operands.
    /// </summary>
    /// <param name="inputFile">The compiled SDT file.</param>
    /// <param name="currentOffset">The offset where to start parsing values.</param>
    /// <returns>String with the encapsulated conditions.</returns>
    private static string EvaluateCondition(byte[] inputFile, ref int currentOffset)
    {
        List<string> conditions = []; // Each comparison or standalone operand
        List<string> joins = [];  // Joins are basically the "&&" (AND) and "||" (OR) between each group of conditions

        byte boundaryOperator;
        do
        {
            string operand_1 = ReadOperand(inputFile, ref currentOffset);   //First get the nested conditions, or the first value in the comparison
            byte operator_ = inputFile[currentOffset++];    //Get the type of operator: relational (1-6) or boundary (0/7/8)
            
            string condition;   //Store as a string tha particular "if" condition
            byte closingOperator;

            if (operator_ >= 0x1 && operator_ <= 0x6)   //Relational comparison closes this group: (operand_1 <operator> operand_2)
            {
                string operand_2 = ReadOperand(inputFile, ref currentOffset);   //Now get the second value, or the nested conditions inside this operand
                condition = operand_1 + GetRelationalSymbol(operator_) + operand_2; //Write the condition
                closingOperator = inputFile[currentOffset++]; //The actual boundary byte (0/7/8)
            }
            else    //No relational operator attached, so the operand is treated as a standalone boolean
            {
                condition = operand_1;
                closingOperator = operator_;
            }

            conditions.Add(condition);
            boundaryOperator = closingOperator;

            if (boundaryOperator == 0x7) joins.Add("&&");   //AND join
            else if (boundaryOperator == 0x8) joins.Add("||");  //OR join
            //If the boundaryOperator is 0x00, it's the terminator
        }
        while(boundaryOperator != 0x0); //Keep looping until we the the terminator operator

        var finalCondition = new StringBuilder(conditions[0]);  //Prepare the string that will contain all of the conditions (first add the first condition to the list)
        for (int i = 1; i < conditions.Count; i++)
            finalCondition.Append(' ').Append(joins[i - 1]).Append(' ').Append(conditions[i]);  //Include a space in between conditions for the joins

        return finalCondition.ToString();

    }

    /// <summary>
    /// For the if and while statements, it is needed to check if the condition is nested or not.
    /// </summary>
    /// <returns>The value itself or the nested condition.</returns>
    private static string ReadOperand(byte[] inputFile, ref int currentOffset)
    {
        bool isNestedCondition = inputFile[currentOffset++] == 0x1;

        return isNestedCondition
            ? "(" + EvaluateCondition(inputFile, ref currentOffset) + ")"
            : GetNumericParameter(inputFile, ref currentOffset);
    }

    /// <summary>
    /// Gets the corresponding operator according to the given byte.
    /// </summary>
    /// <returns>The C operator as a string.</returns>
    /// <exception cref="InvalidDataException">The given operator is not a valid one.</exception>
    private static string GetRelationalSymbol(byte operator_) => operator_ switch
    {
        0x1 => "==",
        0x2 => "!=",
        0x3 => "<",
        0x4 => ">",
        0x5 => "<=",
        0x6 => ">=",
        _ => throw new InvalidDataException("Not a relational operator: " + operator_),
    };

    private static string GetVirtualKeyCode_Constant(byte vk_code) => vk_code switch
    {
        0x01 => "VK_LBUTTON",
        0x02 => "VK_RBUTTON",
        0x03 => "VK_CANCEL",
        0x04 => "VK_MBUTTON",
        0x05 => "VK_XBUTTON1",
        0x06 => "VK_XBUTTON2",
        //0x07: reserved
        0x08 => "VK_BACK",
        0x09 => "VK_TAB",
        //0x0A-0x0B: reserved
        0x0C => "VK_CLEAR",
        0x0D => "VK_RETURN",
        //0x0E-0x0F: undefined
        0x10 => "VK_SHIFT",
        0x11 => "VK_CONTROL",
        0x12 => "VK_MENU",
        0x13 => "VK_PAUSE",
        0x14 => "VK_CAPITAL",
        0x15 => "VK_KANA",
        //0x16: undefined
        0x17 => "VK_JUNJA",
        0x18 => "VK_FINAL",
        0x19 => "VK_KANJI",
        //0x1A: undefined
        0x1B => "VK_ESCAPE",
        0x1C => "VK_CONVERT",
        0x1D => "VK_NONCONVERT",
        0x1E => "VK_ACCEPT",
        0x1F => "VK_MODECHANGE",
        0x20 => "VK_SPACE",
        0x21 => "VK_PRIOR",
        0x22 => "VK_NEXT",
        0x23 => "VK_END",
        0x24 => "VK_HOME",
        0x25 => "VK_LEFT",
        0x26 => "VK_UP",
        0x27 => "VK_RIGHT",
        0x28 => "VK_DOWN",
        0x29 => "VK_SELECT",
        0x2A => "VK_PRINT",
        0x2B => "VK_EXECUTE",
        0x2C => "VK_SNAPSHOT",
        0x2D => "VK_INSERT",
        0x2E => "VK_DELETE",
        0x2F => "VK_HELP",
        0x30 => "VK_0",
        0x31 => "VK_1",
        0x32 => "VK_2",
        0x33 => "VK_3",
        0x34 => "VK_4",
        0x35 => "VK_5",
        0x36 => "VK_6",
        0x37 => "VK_7",
        0x38 => "VK_8",
        0x39 => "VK_9",
        //0x3A-0x40: undefined
        0x41 => "VK_A",
        0x42 => "VK_B",
        0x43 => "VK_C",
        0x44 => "VK_D",
        0x45 => "VK_E",
        0x46 => "VK_F",
        0x47 => "VK_G",
        0x48 => "VK_H",
        0x49 => "VK_I",
        0x4A => "VK_J",
        0x4B => "VK_K",
        0x4C => "VK_L",
        0x4D => "VK_M",
        0x4E => "VK_N",
        0x4F => "VK_O",
        0x50 => "VK_P",
        0x51 => "VK_Q",
        0x52 => "VK_R",
        0x53 => "VK_S",
        0x54 => "VK_T",
        0x55 => "VK_U",
        0x56 => "VK_V",
        0x57 => "VK_W",
        0x58 => "VK_X",
        0x59 => "VK_Y",
        0x5A => "VK_Z",
        0x5B => "VK_LWIN",
        0x5C => "VK_RWIN",
        0x5D => "VK_APPS",
        //0x5E: reserved
        0x5F => "VK_SLEEP",
        0x60 => "VK_NUMPAD0",
        0x61 => "VK_NUMPAD1",
        0x62 => "VK_NUMPAD2",
        0x63 => "VK_NUMPAD3",
        0x64 => "VK_NUMPAD4",
        0x65 => "VK_NUMPAD5",
        0x66 => "VK_NUMPAD6",
        0x67 => "VK_NUMPAD7",
        0x68 => "VK_NUMPAD8",
        0x69 => "VK_NUMPAD9",
        0x6A => "VK_MULTIPLY",
        0x6B => "VK_ADD",
        0x6C => "VK_SEPARATOR",
        0x6D => "VK_SUBTRACT",
        0x6E => "VK_DECIMAL",
        0x6F => "VK_DIVIDE",
        0x70 => "VK_F1",
        0x71 => "VK_F2",
        0x72 => "VK_F3",
        0x73 => "VK_F4",
        0x74 => "VK_F5",
        0x75 => "VK_F6",
        0x76 => "VK_F7",
        0x77 => "VK_F8",
        0x78 => "VK_F9",
        0x79 => "VK_F10",
        0x7A => "VK_F11",
        0x7B => "VK_F12",
        0x7C => "VK_F13",
        0x7D => "VK_F14",
        0x7E => "VK_F15",
        0x7F => "VK_F16",
        0x80 => "VK_F17",
        0x81 => "VK_F18",
        0x82 => "VK_F19",
        0x83 => "VK_F20",
        0x84 => "VK_F21",
        0x85 => "VK_F22",
        0x86 => "VK_F23",
        0x87 => "VK_F24",
        //0x88-0x8F: reserved
        0x90 => "VK_NUMLOCK",
        0x91 => "VK_SCROLL",
        //0x92-0x96: OEM specific
        //0x97-0x9F: unassigned
        0xA0 => "VK_LSHIFT",
        0xA1 => "VK_RSHIFT",
        0xA2 => "VK_LCONTROL",
        0xA3 => "VK_RCONTROL",
        0xA4 => "VK_LMENU",
        0xA5 => "VK_RMENU",
        0xA6 => "VK_BROWSER_BACK",
        0xA7 => "VK_BROWSER_FORWARD",
        0xA8 => "VK_BROWSER_REFRESH",
        0xA9 => "VK_BROWSER_STOP",
        0xAA => "VK_BROWSER_SEARCH",
        0xAB => "VK_BROWSER_FAVORITES",
        0xAC => "VK_BROWSER_HOME",
        0xAD => "VK_VOLUME_MUTE",
        0xAE => "VK_VOLUME_DOWN",
        0xAF => "VK_VOLUME_UP",
        0xB0 => "VK_MEDIA_NEXT_TRACK",
        0xB1 => "VK_MEDIA_PREV_TRACK",
        0xB2 => "VK_MEDIA_STOP",
        0xB3 => "VK_MEDIA_PLAY_PAUSE",
        0xB4 => "VK_LAUNCH_MAIL",
        0xB5 => "VK_LAUNCH_MEDIA_SELECT",
        0xB6 => "VK_LAUNCH_APP1",
        0xB7 => "VK_LAUNCH_APP2",
        //0xB8-0xB9: reserved
        0xBA => "VK_OEM_1",         //Varies per keyboard region
        0xBB => "VK_OEM_PLUS",      //'+' '='
        0xBC => "VK_OEM_COMMA",     //','
        0xBD => "VK_OEM_MINUS",     //'-'
        0xBE => "VK_OEM_PERIOD",    //'.'
        0xBF => "VK_OEM_2",         //Varies per keyboard region
        0xC0 => "VK_OEM_3",         //Varies per keyboard region
        0xC3 => "VK_GAMEPAD_A",
        0xC4 => "VK_GAMEPAD_B",
        0xC5 => "VK_GAMEPAD_X",
        0xC6 => "VK_GAMEPAD_Y",
        0xC7 => "VK_GAMEPAD_RIGHT_SHOULDER",
        0xC8 => "VK_GAMEPAD_LEFT_SHOULDER",
        0xC9 => "VK_GAMEPAD_LEFT_TRIGGER",
        0xCA => "VK_GAMEPAD_RIGHT_TRIGGER",
        0xCB => "VK_GAMEPAD_DPAD_UP",
        0xCC => "VK_GAMEPAD_DPAD_DOWN",
        0xCD => "VK_GAMEPAD_DPAD_LEFT",
        0xCE => "VK_GAMEPAD_DPAD_RIGHT",
        0xCF => "VK_GAMEPAD_MENU",
        0xD0 => "VK_GAMEPAD_VIEW",
        0xD1 => "VK_GAMEPAD_LEFT_THUMBSTICK_BUTTON",
        0xD2 => "VK_GAMEPAD_RIGHT_THUMBSTICK_BUTTON",
        0xD3 => "VK_GAMEPAD_LEFT_THUMBSTICK_UP",
        0xD4 => "VK_GAMEPAD_LEFT_THUMBSTICK_DOWN",
        0xD5 => "VK_GAMEPAD_LEFT_THUMBSTICK_RIGHT",
        0xD6 => "VK_GAMEPAD_LEFT_THUMBSTICK_LEFT",
        0xD7 => "VK_GAMEPAD_RIGHT_THUMBSTICK_UP",
        0xD8 => "VK_GAMEPAD_RIGHT_THUMBSTICK_DOWN",
        0xD9 => "VK_GAMEPAD_RIGHT_THUMBSTICK_RIGHT",
        0xDA => "VK_GAMEPAD_RIGHT_THUMBSTICK_LEFT",
        0xDB => "VK_OEM_4",         //Varies per keyboard region
        0xDC => "VK_OEM_5",         //Varies per keyboard region
        0xDD => "VK_OEM_6",         //Varies per keyboard region
        0xDE => "VK_OEM_7",         //Varies per keyboard region
        0xDF => "VK_OEM_8",         //Varies per keyboard region
        //0xE0: reserved
        //0xE1: OEM specific
        0xE2 => "VK_OEM_102",       //Varies per keyboard region
        //0xE3-0xE4: OEM specific
        0xE5 => "VK_PROCESSKEY",
        //0xE6: OEM specific
        0xE7 => "VK_PACKET",
        //0xE8: unassigned
        //0xE9-0xF5: OEM specific
        0xF6 => "VK_ATTN",
        0xF7 => "VK_CRSEL",
        0xF8 => "VK_EXSEL",
        0xF9 => "VK_EREOF",
        0xFA => "VK_PLAY",
        0xFB => "VK_ZOOM",
        0xFC => "VK_NONAME",
        0xFD => "VK_PA1",
        0xFE => "VK_OEM_CLEAR",
        _ => throw new InvalidDataException("Not a valid Virtual-Key code (or OEM specific one): " + vk_code),
    };
}