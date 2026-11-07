using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;

internal class SDT
{
    internal class SDTScheme
    {
        public uint Opcodes_Offset { get; set; }
        public uint Unknown1_Count { get; set; }
        public uint Unknown2_Count { get; set; }
        public uint Label_Count { get; set; }
        public uint SDTKey_Offset { get; set; }
        public List<SDT_LabelScheme>? SDT_Label { get; set; }   //There must be Label_Count amount
        public byte[] SDTKey { get; set; } = [];    //While the SDT key can be anywhere, the developers just put it after the list of labels (if it exists)
        public byte[] Data { get; set; } = [];  //All of the opcode data
    }

    /// <summary>
    /// For the jump labels, the game keeps a list of each possible label, and then
    /// next to it the offset relative to the opcode region.
    /// </summary>
    internal class SDT_LabelScheme
    {
        public string Name { get; set; } = "";
        public uint Offset { get; set; }
    }

    internal class SDT_Opcode
    {
        public string Name { get; set; } = "";
        
        //What comes next is the next data relative to each opcode, but it's not always the same, so at the moment
        //it'll be kept incomplete for the time being
    }

    public SDTScheme SdtScheme { get; private set; } = new SDTScheme();

    public void ParsePacked(byte[] inputFile)
    {
        
    }
    private void ParseOpcode(ref byte[] inputData, ref int currentOffset)
    {
        SDT_Opcode opcode = new();

        int currentOpcode = BinaryPrimitives.ReadInt32LittleEndian(inputData.AsSpan(currentOffset, 4));

        switch (currentOpcode)
        {
            case 0x2:
                {
                    opcode.Name = "DebugMessage";
                    break;
                }
            case 0x4:
                {
                    opcode.Name = "GetSysModeOn";
                    break;
                }
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
                    opcode.Name = "SetDisplay";
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
            case 0xA0E:
                {
                    opcode.Name = "SetKeyInput";
                    break;
                }
            case 0xA0F:
                {
                    opcode.Name = "SetKeyInputCallLabel";
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
            case 0xA1D:
                {
                    opcode.Name = "ProcPriorityON";
                    break;
                }
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
                    opcode.Name = "if{";
                    break;
                }
            case 0xA3E:
                {
                    opcode.Name = "else{";
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
                    opcode.Name = "GOTO";
                    break;
                }
            case 0xA43:
                {
                    opcode.Name = "Call";
                    break;
                }
            case 0xA44:
                {
                    opcode.Name = "FarCallLabel";
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
                    opcode.Name = "Jump";
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
            case 0xA6F:
                {
                    opcode.Name = "MacroPriority_Set";
                    break;
                }
            case 0xA70:
                {
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
                    opcode.Name = "[] = Num";
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
                    opcode.Name = "SetStr";
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
            case 0xD00:
                {
                    opcode.Name = "Disp";
                    break;
                }
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
            case 0xD08:
                {
                    opcode.Name = "LoadOBJ";
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
            case 0xD1D:
                {
                    opcode.Name = "ObjPosDest";
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
            default:
                throw new InvalidDataException("Unknown " + currentOpcode + " opcode.");
                
        }
    }
}