using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Turune.Models.Formats.Initialization
{
    internal class DAT
    {
        internal class DATScheme
        {
            //When parsing the DAT file, the game does it in 3 parts, the same is done here, since each
            //part has a different size and structure.
            public DATScheme_Part1 Part1 { get; set; } = new DATScheme_Part1();
            public DATScheme_Part2 Part2 { get; set; } = new DATScheme_Part2();
            public DATScheme_Part3 Part3 { get; set; } = new DATScheme_Part3();
        }

        //0x2194 bytes (hardcoded value)
        internal class DATScheme_Part1
        {
            public DateTime GameIdentifier { get; set; }
            public uint GameSystemMode { get; set; }
            public byte ExclusiveFullscreen { get; set; }
            public byte LocaleEnglish { get; set; }
            public string WindowTitle { get; set; } = "";
            public string DATKey { get; set; } = "";
            public ushort WindowWidth { get; set; }
            public ushort WindowHeight { get; set; }
            public byte[] Reserved0 { get; set; } = [0x00, 0x00];

            //Flags
            public int DI_Flags { get; set; }
            public int DG_Flags { get; set; }
            public int DT_Flags { get; set; }
            public int SI_Flags { get; set; }
            public int SG_Flags { get; set; }
            public int ST_Flags { get; set; }

            //Slots
            public int Obj_Slots { get; set; }
            public int MSK_Slots { get; set; }
            public int CGB_Slots { get; set; }
            public int Button_Slots { get; set; }
            public int Box_Slots { get; set; }
            public int MessageLog_Slots { get; set; }

            //obj IDs
            public int ObjID_MessageWindow { get; set; }
            public int ObjID_MaxValueOnBoot { get; set; }

            //Folder names
            public string SDT_FolderName { get; set; } = "data06";
            public string MG2_FolderName { get; set; } = "data00";
            public string MAL_FolderName { get; set; } = "data02";
            public string BGM_FolderName { get; set; } = "data05";
            public string Voice_FolderName { get; set; } = "data03";
            public string SE_FolderName { get; set; } = "data04";
            public string AM2_FolderName { get; set; } = "data01";
            public string Unused_FolderName { get; set; } = "";

            //Save files
            public string SaveData_FolderName { get; set; } = "data";
            public string SaveData_FileName { get; set; } = "Flag.odn";

            //Close game process
            public byte CloseGame_UseMessageBox { get; set; }
            public byte Reserved1 { get; set; } = 0;
            public ushort CloseGame_SeenNumber { get; set; }
            public string CloseGame_JumpLabel { get; set; } = "";

            //Screen mode process
            public uint ScreenMode_ActivateFullScreen_SeenNumber { get; set; }
            public string ScreenMode_ActivateFullScreen_JumpLabel { get; set; } = "";
            public uint ScreenMode_ActivateWindowed_SeenNumber { get; set; }
            public string ScreenMode_ActivateWindowed_JumpLabel { get; set; } = "";

            //Fonts
            public string FontName { get; set; } = "ＭＳ ゴシック";
            public DATScheme_Font[] Font { get; set; } = DefaultFonts();  //50 fonts in total

            //Message window: color customization
            public string[] MessageWindow_CustomColorCharName_CharName { get; set; } = new string[50];
            public byte[][] MessageWindow_CustomColorCharName_RGB { get; set; } = new byte[50][];
            public byte[] MessageWindow_CharacterName_DefaultRGB { get; set; } = [0xFF, 0xFF, 0xFF];
            public byte MessageWindow_CustomColorCharName_Enable { get; set; } = 1;
            public byte[] MessageWindow_Message_DefaultRGB { get; set; } = [0xFF, 0xFF, 0xFF];

            //Background fill
            public byte[] BackgroundFill_RGB { get; set; } = [0x00, 0x00, 0x00];
            public byte BackgroundFill_Opacity { get; set; } = 0xC8;

            //Reserved: some of these values get copied directly to the save game file, but they are not used by the game in any way.
            public byte ReservedSaveGame0 { get; set; } = 1;
            public byte ReservedSaveGame1 { get; set; } = 1;
            public byte Reserved2 { get; set; } = 0;
            public byte[] ReservedSaveGame2 { get; set; } = [0xA, 0x00];

            //Message window: font used and order of the RECTS
            public ushort MessageWindow_MessagePart_FontID { get; set; } = 0;
            public ushort MessageWindow_RECTOrder_SurfaceImage { get; set; } = 0;
            public ushort MessageWindow_RECTOrder_Image { get; set; } = 1;
            public ushort MessageWindow_RECTOrder_SurfaceMessageText { get; set; } = 2;
            public ushort MessageWindow_RECTOrder_SurfaceCharacterNameText { get; set; } = 3;
            public ushort MessageWindow_RECTOrder_MessageText { get; set; } = 4;
            public ushort MessageWindow_RECTOrder_CharacterNameText { get; set; } = 5;

            //Choice boxes: obj IDs slots
            public ushort ChoiceBox_FirstID { get; set; } = 75;
            public ushort ChoiceBox_LastID { get; set; } = 99;

            //Audio instances
            public ushort AudioInstances_TotalAmount { get; set; } = 25;
            public ushort AudioInstances_BGM { get; set; } = 0;
            public ushort AudioInstances_Voice { get; set; } = 1;
            public byte AudioInstances_Unused { get; set; } = 50;

            //SDT key
            public byte SDTKey_Shuffler { get; set; } = 0;
            public string SDTKey { get; set; } = "";

            //Methods
            private static DATScheme_Font[] DefaultFonts()
            {
                var DefaultFonts = new DATScheme_Font[50];
                for (int i = 0; i < 50; i++) DefaultFonts[i] = new DATScheme_Font();

                //Setting up the fonts the game actually uses and modifies its values
                DefaultFonts[0].Height = 24;
                DefaultFonts[0].DropShadow_RGBA = [0x64, 0x00, 0x00, 0x00];
                DefaultFonts[1].Height = 31;
                DefaultFonts[1].DropShadow_RGBA = [0x64, 0x00, 0x00, 0x00];
                return DefaultFonts;
            }
        }

        //0x37F0 bytes (hardcoded value)
        internal class DATScheme_Part2
        {
            //Message window: message part background fill and RECTs
            public byte MessageWindow_MessagePart_BackgroundFill_Enable { get; set; } = 0;
            public byte Reserved0 { get; set; } = 0;
            public short MessageWindow_MessagePart_Background_X { get; set; } = 128;
            public short MessageWindow_MessagePart_Background_Y { get; set; } = 546;
            public short MessageWindow_MessagePart_Text_X { get; set; } = 308;
            public short MessageWindow_MessagePart_Text_Y { get; set; } = 587;
            public ushort MessageWindow_MessagePart_Text_Width { get; set; } = 722;
            public ushort MessageWindow_MessagePart_Text_Height { get; set; } = 110;
            public short MessageWindow_MessagePart_BackgroundFill_X { get; set; } = 128;
            public short MessageWindow_MessagePart_BackgroundFill_Y { get; set; } = 556;
            public ushort MessageWindow_MessagePart_BackgroundFill_Width { get; set; } = 0;
            public ushort MessageWindow_MessagePart_BackgroundFill_Height { get; set; } = 0;
            public string MessageWindow_MessagePart_Background_ImageFile { get; set; } = "メッセージウインドウ01.mg2";

            //Message window: character name part
            public ushort MessageWindow_CharacterNamePart_FontID { get; set; } = 0;
            public byte MessageWindow_CharacterNamePart_BackgroundFill_Enable { get; set; } = 0;
            public byte Reserved1 { get; set; } = 0;
            public short MessageWindow_CharacterNamePart_Background_X { get; set; } = 128;
            public short MessageWindow_CharacterNamePart_Background_Y { get; set; } = 546;
            public short MessageWindow_CharacterNamePart_Text_X { get; set; } = 283;
            public short MessageWindow_CharacterNamePart_Text_Y { get; set; } = 562;
            public ushort MessageWindow_CharacterNamePart_Text_Width { get; set; } = 600;
            public ushort MessageWindow_CharacterNamePart_Text_Height { get; set; } = 35;
            public short MessageWindow_CharacterNamePart_BackgroundFill_X { get; set; } = 0;
            public short MessageWindow_CharacterNamePart_BackgroundFill_Y { get; set; } = 0;
            public ushort MessageWindow_CharacterNamePart_BackgroundFill_Width { get; set; } = 0;
            public ushort MessageWindow_CharacterNamePart_BackgroundFill_Height { get; set; } = 0;
            public ushort MessageWindow_CharacterNamePart_Unknown1 { get; set; } = 600;
            public ushort MessageWindow_CharacterNamePart_Unknown2 { get; set; } = 0;
            public byte MessageWindow_CharacterNamePart_Unknown3 { get; set; } = 0;
            public string MessageWindow_CharacterNamePart_Background_ImageFile { get; set; } = "null.mg2";

            //Reserved
            public byte Reserved2 = 0;
            public byte[] Reserved3 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved4 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved5 = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved6 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved7 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved8 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved9 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved10 = 
                [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved11 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved12 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved13 = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved14 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved15 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved16 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved17 = [0x00, 0x00, 0x00, 0x00];

            //The remaining 13644 bytes are simply unassigned to any variable, so they have to be filled with 0's
        }

        //0x6A90 bytes (hardcoded value)
        internal class DATScheme_Part3
        {
            //Choice boxes: main parameters
            public byte ChoiceBox_Text_Alignment { get; set; } = 4;
            public byte Reserved0 { get; set; } = 0;
            public ushort ChoiceBox_Text_FontID { get; set; } = 0;
            public byte[] ChoiceBox_Text_RGB { get; set; } = [0xFF, 0xFF, 0xFF];
            public byte ChoiceBox_BackgroundFill_Enable { get; set; } = 0;

            //Choice boxes: RECT positions
            public short ChoiceBox_Text_RECT_X { get; set; } = 135;
            public short ChoiceBox_Text_RECT_Y { get; set; } = 11;
            public ushort ChoiceBox_Text_RECT_Width { get; set; } = 600;
            public ushort ChoiceBox_Text_RECT_Height { get; set; } = 47;
            public ushort ChoiceBox_Unknown1 { get; set; } = 0;
            public ushort ChoiceBox_Unknown2 { get; set; } = 0;
            public ushort ChoiceBox_Unknown3 { get; set; } = 660;
            public ushort ChoiceBox_BackgroundFill_Height { get; set; } = 47;
            public ushort ChoiceBox_BackgroundFill_Width { get; set; } = 660;
            public short ChoiceBox_Unknown4 { get; set; } = 47;
            public short ChoiceBox_Image_YMargin { get; set; } = 65;
            public short ChoiceBox_Image_XMargin { get; set; } = 310;
            public short ChoiceBox_Unknown5 { get; set; } = 302;
            public string ChoiceBox_Image_NotChosen { get; set; } = "選択肢枠(基本).mg2";
            public string ChoiceBox_Image_NotChosen_MouseHovered { get; set; } = "選択肢枠(ロール).mg2";
            public string ChoiceBox_Image_AlreadyChosen { get; set; } = "選択肢枠(既読).mg2";
            public string ChoiceBox_Image_AlreadyChosen_MouseHovered { get; set; } = "選択肢枠(ロール).mg2";

            //Unused
            public byte[] Reserved1 { get; set; } = [0x00, 0x00];
            public int Unused0 { get; set; } = 0;
            public int Unused1 { get; set; } = 0;
            public int Unused2 { get; set; } = 0;
            public byte[] Reserved2 { get; set; } = [0x00, 0x00, 0x00, 0x00];
            public int Unused3 { get; set; } = 0;
            public int Unused4 { get; set; } = 0;
            public int Unused5 { get; set; } = 0;
            public int Unused6 { get; set; } = 0;
            public byte[] Reserved3 =
                [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            public int Unused7 { get; set; } = 0;
            public int Unused8 { get; set; } = 0;
            public int Unused9 { get; set; } = 0;
            public byte[] Reserved4 { get; set; } = [0x00, 0x00, 0x00, 0x00];
            public int Unused10 { get; set; } = 0;
            public int Unused11 { get; set; } = 0;
            public int Unused12 { get; set; } = 0;
            public int Unused13 { get; set; } = 0;
            public byte[] Reserved5 =
                [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            public int Unused14 { get; set; } = 0;
            public int Unused15 { get; set; } = 0;
            public byte[] Reserved6 =
                [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            public int Unused16 { get; set; } = 0;
            public int Unused17 { get; set; } = 0;
            public int Unused18 { get; set; } = 0;
            public byte[] Reserved7 =
                [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            public int Unused19 { get; set; } = 0;
            public int Unused20 { get; set; } = 0;
            public byte[] Reserved8 =
                [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00];
            public int Unused21 { get; set; } = 0;
            public int Unused22 { get; set; } = 0;
            public int Unused23 { get; set; } = 0;

            //The remaining 25952 bytes have to be filled with 0's
        }

        internal class DATScheme_Font
        {
            public int Height { get; set; } = 25;
            public int Width { get; set; } = 0;
            public int Weight { get; set; } = 300;
            public int DropShadow_HorizontalOffset { get; set; } = 1;
            public int DropShadow_VerticalOffset { get; set; } = 1;
            public byte[] DropShadow_RGBA { get; set; } = [0x41, 0x00, 0x00, 0x00]; //RGBA is a class present only in .NET 11 and above (https://learn.microsoft.com/en-us/dotnet/api/system.numerics.colors.rgba?view=net-11.0)
            public uint Reserved { get; set; } = 0;
        }

        public DATScheme DatScheme { get; private set; } = new DATScheme();

        ///<summary>Gets the INI scheme from an original system.dat file.</summary>
        ///<param name="inputFile">Original system.dat file.</param>
        public void ParsePacked(byte[] inputFile)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding shiftJIS = Encoding.GetEncoding("shift-jis");

            if (inputFile.Length != 50196) throw new Exception("The INI file does not " +
                "have the correct size, it must be 50196 bytes.");

            int currentOffset = 0;

            ////////////
            // 1st PART
            ////////////
            
            int gameIdentifier = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            DatScheme.Part1.GameIdentifier = new DateTime(1970, 1, 1).AddSeconds(gameIdentifier);
            currentOffset += 4;

            DatScheme.Part1.GameSystemMode = BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            DatScheme.Part1.ExclusiveFullscreen = inputFile[currentOffset];
            currentOffset++;

            DatScheme.Part1.LocaleEnglish = inputFile[currentOffset];
            currentOffset++;

            DatScheme.Part1.WindowTitle = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            DatScheme.Part1.DATKey = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            DatScheme.Part1.WindowWidth = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part1.WindowHeight = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            currentOffset += 2; //DatScheme.Part1.Reserved0

            //Flags
            DatScheme.Part1.DI_Flags = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            DatScheme.Part1.DG_Flags = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            DatScheme.Part1.DT_Flags = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            DatScheme.Part1.SI_Flags = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            DatScheme.Part1.SG_Flags = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            DatScheme.Part1.ST_Flags = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            //Slots
            DatScheme.Part1.Obj_Slots = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            DatScheme.Part1.MSK_Slots = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            DatScheme.Part1.CGB_Slots = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            DatScheme.Part1.Button_Slots = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            DatScheme.Part1.Box_Slots = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            DatScheme.Part1.MessageLog_Slots = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            //obj IDs
            DatScheme.Part1.ObjID_MessageWindow = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            DatScheme.Part1.ObjID_MaxValueOnBoot = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            //Folder names
            DatScheme.Part1.SDT_FolderName = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            DatScheme.Part1.MG2_FolderName = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            DatScheme.Part1.MAL_FolderName = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            DatScheme.Part1.BGM_FolderName = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            DatScheme.Part1.Voice_FolderName = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            DatScheme.Part1.SE_FolderName = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            DatScheme.Part1.AM2_FolderName = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            DatScheme.Part1.Unused_FolderName = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            //Save files
            DatScheme.Part1.SaveData_FolderName = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            DatScheme.Part1.SaveData_FileName = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            //Close game process
            DatScheme.Part1.CloseGame_UseMessageBox = inputFile[currentOffset];
            currentOffset++;

            currentOffset++; //DatScheme.Part1.Reserved1

            DatScheme.Part1.CloseGame_SeenNumber = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part1.CloseGame_JumpLabel = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            //Screen mode process
            DatScheme.Part1.ScreenMode_ActivateFullScreen_SeenNumber = BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            DatScheme.Part1.ScreenMode_ActivateFullScreen_JumpLabel = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            DatScheme.Part1.ScreenMode_ActivateWindowed_SeenNumber = BinaryPrimitives.ReadUInt32LittleEndian(inputFile.AsSpan(currentOffset, 4));
            currentOffset += 4;

            DatScheme.Part1.ScreenMode_ActivateWindowed_JumpLabel = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            //Fonts
            DatScheme.Part1.FontName = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            var Fonts = new DATScheme_Font[50];

            for (int i = 0; i < 50; i++)
            {
                Fonts[i] = new DATScheme_Font
                {
                    Height = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset, 4)),
                    Width = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset + 4, 4)),
                    Weight = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset + 8, 4)),
                    DropShadow_HorizontalOffset = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset + 12, 4)),
                    DropShadow_VerticalOffset = BinaryPrimitives.ReadInt32LittleEndian(inputFile.AsSpan(currentOffset + 16, 4)),
                    DropShadow_RGBA = inputFile.AsSpan(currentOffset + 20, 4).ToArray(),
                };
                currentOffset += 28;
            }

            //Message window: color customization
            for (int i = 0; i < 50; i++)
            {
                DatScheme.Part1.MessageWindow_CustomColorCharName_CharName[i] = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 50)).Replace("\0", "");
                currentOffset += 50;
            }

            for (int i = 0; i < 50; i++)
            {
                DatScheme.Part1.MessageWindow_CustomColorCharName_RGB[i] = inputFile.AsSpan(currentOffset, 3).ToArray();
                currentOffset += 3;
            }

            DatScheme.Part1.MessageWindow_CharacterName_DefaultRGB = inputFile.AsSpan(currentOffset, 3).ToArray();
            currentOffset += 3;

            DatScheme.Part1.MessageWindow_CustomColorCharName_Enable = inputFile[currentOffset];
            currentOffset++;

            DatScheme.Part1.MessageWindow_Message_DefaultRGB = inputFile.AsSpan(currentOffset, 3).ToArray();
            currentOffset += 3;

            //Background fill
            DatScheme.Part1.BackgroundFill_RGB = inputFile.AsSpan(currentOffset, 3).ToArray();
            currentOffset += 3;

            DatScheme.Part1.BackgroundFill_Opacity = inputFile[currentOffset];
            currentOffset++;

            //Reserved
            currentOffset++; //DatScheme.Part1.ReservedSaveGame0
            currentOffset++; //DatScheme.Part1.ReservedSaveGame1
            currentOffset++; //DatScheme.Part1.Reserved2
            currentOffset += 2; //DatScheme.Part1.ReservedSaveGame2

            //Message window: font used and order of the RECTS
            DatScheme.Part1.MessageWindow_MessagePart_FontID = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part1.MessageWindow_RECTOrder_SurfaceImage = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part1.MessageWindow_RECTOrder_Image = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part1.MessageWindow_RECTOrder_SurfaceMessageText = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part1.MessageWindow_RECTOrder_SurfaceCharacterNameText = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part1.MessageWindow_RECTOrder_MessageText = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part1.MessageWindow_RECTOrder_CharacterNameText = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            //Choice boxes: obj IDs slots
            DatScheme.Part1.ChoiceBox_FirstID = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part1.ChoiceBox_LastID = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            //Audio instances
            DatScheme.Part1.AudioInstances_TotalAmount = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part1.AudioInstances_BGM = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part1.AudioInstances_Voice = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part1.AudioInstances_Unused = inputFile[currentOffset];
            currentOffset++;

            //SDT key
            DatScheme.Part1.SDTKey_Shuffler = inputFile[currentOffset];
            currentOffset++;

            byte[] encryptedSDTKey = inputFile.AsSpan(currentOffset, 260).ToArray();    //The SDT key is encrypted from the get-go
            int sdtKey_NullIndex = Array.IndexOf(encryptedSDTKey, (byte)0);     //Since it is a string when decrypted, we have to trim out the null bytes in the end (if there are)
            encryptedSDTKey = encryptedSDTKey.AsSpan(0, sdtKey_NullIndex).ToArray();
            DatScheme.Part1.SDTKey = DecryptSDTKey(encryptedSDTKey, DatScheme.Part1.DATKey);
            currentOffset += 260;


            ////////////
            // 2nd PART
            ////////////

            //Message window: message part background fill and RECTs
            DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Enable = inputFile[currentOffset];
            currentOffset++;

            currentOffset++; //dataScheme.Part2.Reserved0

            DatScheme.Part2.MessageWindow_MessagePart_Background_X = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_MessagePart_Background_Y = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_MessagePart_Text_X = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_MessagePart_Text_Y = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_MessagePart_Text_Width = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_MessagePart_Text_Height = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_X = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Y = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Width = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Height = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_MessagePart_Background_ImageFile = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            //Message window: character name part
            DatScheme.Part2.MessageWindow_CharacterNamePart_FontID = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Enable = inputFile[currentOffset];
            currentOffset++;

            currentOffset++; //DatScheme.Part2.Reserved1

            DatScheme.Part2.MessageWindow_CharacterNamePart_Background_X = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_CharacterNamePart_Background_Y = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_CharacterNamePart_Text_X = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_CharacterNamePart_Text_Y = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_CharacterNamePart_Text_Width = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_CharacterNamePart_Text_Height = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_X = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Y = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Width = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Height = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_CharacterNamePart_Unknown1 = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_CharacterNamePart_Unknown2 = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part2.MessageWindow_CharacterNamePart_Unknown3 = inputFile[currentOffset];
            currentOffset++;

            DatScheme.Part2.MessageWindow_CharacterNamePart_Background_ImageFile = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            //Reserved
            currentOffset++; //DatScheme.Part2.Reserved2
            currentOffset += 4; //DatScheme.Part2.Reserved3
            currentOffset += 4; //DatScheme.Part2.Reserved4
            currentOffset += 8; //DatScheme.Part2.Reserved5
            currentOffset += 4; //DatScheme.Part2.Reserved6
            currentOffset += 4; //DatScheme.Part2.Reserved7
            currentOffset += 4; //DatScheme.Part2.Reserved8
            currentOffset += 4; //DatScheme.Part2.Reserved9
            currentOffset += 40; //DatScheme.Part2.Reserved10
            currentOffset += 4; //DatScheme.Part2.Reserved11
            currentOffset += 4; //DatScheme.Part2.Reserved12
            currentOffset += 8; //DatScheme.Part2.Reserved13
            currentOffset += 4; //DatScheme.Part2.Reserved14
            currentOffset += 4; //DatScheme.Part2.Reserved15
            currentOffset += 4; //DatScheme.Part2.Reserved16
            currentOffset += 4; //DatScheme.Part2.Reserved17

            currentOffset += 13644; //Skip the padding for the 2nd part


            ////////////
            // 3rd PART
            ////////////

            //Choice boxes: main parameters
            DatScheme.Part3.ChoiceBox_Text_Alignment = inputFile[currentOffset];
            currentOffset++;

            currentOffset++; //DatScheme.Part3.Reserved0

            DatScheme.Part3.ChoiceBox_Text_FontID = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part3.ChoiceBox_Text_RGB = inputFile.AsSpan(currentOffset, 3).ToArray();
            currentOffset += 3;

            DatScheme.Part3.ChoiceBox_BackgroundFill_Enable = inputFile[currentOffset];
            currentOffset++;

            //Choice boxes: RECT positions
            DatScheme.Part3.ChoiceBox_Text_RECT_X = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part3.ChoiceBox_Text_RECT_Y = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part3.ChoiceBox_Text_RECT_Width = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part3.ChoiceBox_Text_RECT_Height = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part3.ChoiceBox_Unknown1 = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part3.ChoiceBox_Unknown2 = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part3.ChoiceBox_Unknown3 = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part3.ChoiceBox_BackgroundFill_Height = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part3.ChoiceBox_BackgroundFill_Width = BinaryPrimitives.ReadUInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part3.ChoiceBox_Unknown4 = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part3.ChoiceBox_Image_YMargin = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part3.ChoiceBox_Image_XMargin = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part3.ChoiceBox_Unknown5 = BinaryPrimitives.ReadInt16LittleEndian(inputFile.AsSpan(currentOffset, 2));
            currentOffset += 2;

            DatScheme.Part3.ChoiceBox_Image_NotChosen = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            DatScheme.Part3.ChoiceBox_Image_NotChosen_MouseHovered = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            DatScheme.Part3.ChoiceBox_Image_AlreadyChosen = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            DatScheme.Part3.ChoiceBox_Image_AlreadyChosen_MouseHovered = shiftJIS.GetString(inputFile.AsSpan(currentOffset, 260)).Replace("\0", "");
            currentOffset += 260;

            //Unused
            currentOffset += 2; //DatScheme.Part3.Reserved1
            currentOffset += 4; //DatScheme.Part3.Unused0
            currentOffset += 4; //DatScheme.Part3.Unused1
            currentOffset += 4; //DatScheme.Part3.Unused2
            currentOffset += 4; //DatScheme.Part3.Reserved2
            currentOffset += 4; //DatScheme.Part3.Unused3
            currentOffset += 4; //DatScheme.Part3.Unused4
            currentOffset += 4; //DatScheme.Part3.Unused5
            currentOffset += 4; //DatScheme.Part3.Unused6
            currentOffset += 40; //DatScheme.Part3.Reserved3
            currentOffset += 4; //DatScheme.Part3.Unused7
            currentOffset += 4; //DatScheme.Part3.Unused8
            currentOffset += 4; //DatScheme.Part3.Unused9
            currentOffset += 4; //DatScheme.Part3.Reserved4
            currentOffset += 4; //DatScheme.Part3.Unused10
            currentOffset += 4; //DatScheme.Part3.Unused11
            currentOffset += 4; //DatScheme.Part3.Unused12
            currentOffset += 4; //DatScheme.Part3.Unused13
            currentOffset += 40; //DatScheme.Part3.Reserved5
            currentOffset += 4; //DatScheme.Part3.Unused14
            currentOffset += 4; //DatScheme.Part3.Unused15
            currentOffset += 16; //DatScheme.Part3.Reserved6
            currentOffset += 4; //DatScheme.Part3.Unused16
            currentOffset += 4; //DatScheme.Part3.Unused17
            currentOffset += 4; //DatScheme.Part3.Unused18
            currentOffset += 40; //DatScheme.Part3.Reserved7
            currentOffset += 4; //DatScheme.Part3.Unused19
            currentOffset += 4; //DatScheme.Part3.Unused20
            currentOffset += 12; //DatScheme.Part3.Reserved8
            currentOffset += 4; //DatScheme.Part3.Unused21
            currentOffset += 4; //DatScheme.Part3.Unused22
            currentOffset += 4; //DatScheme.Part3.Unused23

            currentOffset += 25952;

        }

        ///<summary>Gets the DAT scheme from a normal INI file.</summary>
        ///<param name="inputLines">DAT file as an array of strings.</param>
        public void ParseUnpacked(string[] inputLines)
        {
            var iniReader = new Utils.IniFile();
            iniReader.Parse(inputLines);

            //Sanity check to ensure that the game has the fundamental sections
            string[] requiredSections =
            {
                "General", "Security", "Flags", "Slots", "Object_ID", "FolderNames", "SaveGames",
                "Process_CloseGame", "Process_ScreenMode", "Fonts", "Fonts_CharacterNameCustomColor",
                "BackgroundFill", "Fonts_ID", "MessageWindow_LayerOrder", "Slots_ChoiceBoxes", "Audio",
                "MessageWindow_Message", "MessageWindow_CharacterName", "ChoiceBoxes"
            };
            
            string[] missingSections = requiredSections.Where(section => !iniReader.SectionExists(section)).ToArray();

            if (missingSections.Length > 0)
                throw new Exception($"The INI file is missing the following sections: {string.Join(", ", missingSections)}");

            //General
            DatScheme.Part1.GameIdentifier = Convert.ToDateTime(iniReader.Read("General", "GameIdentifier"));
            DatScheme.Part1.GameSystemMode = Convert.ToUInt32(iniReader.Read("General", "GameSystemMode"));
            DatScheme.Part1.ExclusiveFullscreen = Convert.ToByte(iniReader.Read("General", "ExclusiveFullscreen"));
            DatScheme.Part1.LocaleEnglish = Convert.ToByte(iniReader.Read("General", "LocaleEnglish"));
            DatScheme.Part1.WindowTitle = iniReader.Read("General", "WindowTitle");
            DatScheme.Part1.WindowWidth = Convert.ToUInt16(iniReader.Read("General", "WindowWidth"));
            DatScheme.Part1.WindowHeight = Convert.ToUInt16(iniReader.Read("General", "WindowHeight"));

            //Security
            DatScheme.Part1.DATKey = iniReader.Read("Security", "DATKey");
            DatScheme.Part1.SDTKey = iniReader.Read("Security", "SDTKey");

            //Flags
            DatScheme.Part1.DI_Flags = Convert.ToInt32(iniReader.Read("Flags", "DI"));
            DatScheme.Part1.DG_Flags = Convert.ToInt32(iniReader.Read("Flags", "DG"));
            DatScheme.Part1.DT_Flags = Convert.ToInt32(iniReader.Read("Flags", "DT"));
            DatScheme.Part1.SI_Flags = Convert.ToInt32(iniReader.Read("Flags", "SI"));
            DatScheme.Part1.SG_Flags = Convert.ToInt32(iniReader.Read("Flags", "SG"));
            DatScheme.Part1.ST_Flags = Convert.ToInt32(iniReader.Read("Flags", "ST"));

            //Slots
            DatScheme.Part1.Obj_Slots = Convert.ToInt32(iniReader.Read("Slots", "Objects"));
            DatScheme.Part1.MSK_Slots = Convert.ToInt32(iniReader.Read("Slots", "Mask"));
            DatScheme.Part1.CGB_Slots = Convert.ToInt32(iniReader.Read("Slots", "CGB"));
            DatScheme.Part1.Button_Slots = Convert.ToInt32(iniReader.Read("Slots", "Button"));
            DatScheme.Part1.Box_Slots = Convert.ToInt32(iniReader.Read("Slots", "Box"));
            DatScheme.Part1.MessageLog_Slots = Convert.ToInt32(iniReader.Read("Slots", "MessageLog"));

            //Object_ID
            DatScheme.Part1.ObjID_MessageWindow = Convert.ToInt32(iniReader.Read("Object_ID", "MessageWindow"));
            DatScheme.Part1.ObjID_MaxValueOnBoot = Convert.ToInt32(iniReader.Read("Object_ID", "MaxValueOnBoot"));

            //FolderNames
            DatScheme.Part1.SDT_FolderName = iniReader.Read("FolderNames", "SDT");
            DatScheme.Part1.MG2_FolderName = iniReader.Read("FolderNames", "MG2");
            DatScheme.Part1.MAL_FolderName = iniReader.Read("FolderNames", "MAL");
            DatScheme.Part1.BGM_FolderName = iniReader.Read("FolderNames", "BGM");
            DatScheme.Part1.Voice_FolderName = iniReader.Read("FolderNames", "Voice");
            DatScheme.Part1.SE_FolderName = iniReader.Read("FolderNames", "SE");
            DatScheme.Part1.AM2_FolderName = iniReader.Read("FolderNames", "AM2");
            DatScheme.Part1.Unused_FolderName = iniReader.Read("FolderNames", "Unused");

            //SaveGames
            DatScheme.Part1.SaveData_FolderName = iniReader.Read("SaveGames", "FolderName");
            DatScheme.Part1.SaveData_FileName = iniReader.Read("SaveGames", "SaveName");

            //Process_CloseGame
            DatScheme.Part1.CloseGame_UseMessageBox = Convert.ToByte(iniReader.Read("Process_CloseGame", "UseMessageBox"));
            DatScheme.Part1.CloseGame_SeenNumber = Convert.ToUInt16(iniReader.Read("Process_CloseGame", "SeenNumber"));
            DatScheme.Part1.CloseGame_JumpLabel = iniReader.Read("Process_CloseGame", "JumpLabel");

            //Process_ScreenMode
            DatScheme.Part1.ScreenMode_ActivateFullScreen_SeenNumber = Convert.ToUInt16(iniReader.Read("Process_ScreenMode", "FullScreen_SeenNumber"));
            DatScheme.Part1.ScreenMode_ActivateFullScreen_JumpLabel = iniReader.Read("Process_ScreenMode", "FullScreen_JumpLabel");
            DatScheme.Part1.ScreenMode_ActivateWindowed_SeenNumber = Convert.ToUInt16(iniReader.Read("Process_ScreenMode", "Windowed_SeenNumber"));
            DatScheme.Part1.ScreenMode_ActivateWindowed_JumpLabel = iniReader.Read("Process_ScreenMode", "Windowed_JumpLabel");

            //Font
            DatScheme.Part1.FontName = iniReader.Read("Fonts", "Name");
            string color_CharacterName = iniReader.Read("Fonts", "Color_CharacterName").TrimStart('#');
            DatScheme.Part1.MessageWindow_CharacterName_DefaultRGB = new byte[]
            {
                Convert.ToByte(color_CharacterName.Substring(4, 2), 16), // B
                Convert.ToByte(color_CharacterName.Substring(2, 2), 16), // G
                Convert.ToByte(color_CharacterName.Substring(0, 2), 16)  // R
            };
            string color_MessageText = iniReader.Read("Fonts", "Color_MessageText").TrimStart('#');
            DatScheme.Part1.MessageWindow_Message_DefaultRGB = new byte[]
            {
                Convert.ToByte(color_MessageText.Substring(4, 2), 16), // B
                Convert.ToByte(color_MessageText.Substring(2, 2), 16), // G
                Convert.ToByte(color_MessageText.Substring(0, 2), 16)  // R
            };
            string color_ChoiceBoxText = iniReader.Read("Fonts", "ChoiceBox_Text_RGB").TrimStart('#');
            DatScheme.Part3.ChoiceBox_Text_RGB = new byte[]
            {
                Convert.ToByte(color_ChoiceBoxText.Substring(4, 2), 16), //B
                Convert.ToByte(color_ChoiceBoxText.Substring(2, 2), 16), //G
                Convert.ToByte(color_ChoiceBoxText.Substring(0, 2), 16)  //R
            };
            for (int i = 0; i < DatScheme.Part1.Font.Length; i++)
            {
                DatScheme.Part1.Font[i].Height = Convert.ToInt32(iniReader.Read($"Fonts_{i}", "Height"));
                DatScheme.Part1.Font[i].Width = Convert.ToInt32(iniReader.Read($"Fonts_{i}", "Width"));
                DatScheme.Part1.Font[i].Weight = Convert.ToInt32(iniReader.Read($"Fonts_{i}", "Weight"));
                DatScheme.Part1.Font[i].DropShadow_HorizontalOffset = Convert.ToInt32(iniReader.Read($"Fonts_{i}", "DropShadow_HorizontalOffset"));
                DatScheme.Part1.Font[i].DropShadow_VerticalOffset = Convert.ToInt32(iniReader.Read($"Fonts_{i}", "DropShadow_VerticalOffset"));
                string color_DropShadow = iniReader.Read($"Fonts_{i}", "DropShadow_RGBA").TrimStart('#');
                DatScheme.Part1.Font[i].DropShadow_RGBA = new byte[]
                {
                    Convert.FromHexString(color_DropShadow)[3], //A
                    Convert.FromHexString(color_DropShadow)[2], //B
                    Convert.FromHexString(color_DropShadow)[1], //G
                    Convert.FromHexString(color_DropShadow)[0], //R
                };
            }

            //Fonts_CharacterNameCustomColor
            DatScheme.Part1.MessageWindow_CustomColorCharName_Enable = Convert.ToByte(iniReader.Read("Fonts_CharacterNameCustomColor", "Enabled"));
            for (int i = 0; i < DatScheme.Part1.Font.Length; i++)
            {
                iniReader.Read($"Fonts_CharacterNameCustomColor_{i}", "CharacterName");
                string color_CustomCharacterName = iniReader.Read($"Fonts_CharacterNameCustomColor_{i}", "Color").TrimStart('#');
                DatScheme.Part1.MessageWindow_CustomColorCharName_RGB[i] = new byte[]
                {
                Convert.ToByte(color_CustomCharacterName.Substring(4, 2), 16), // B
                Convert.ToByte(color_CustomCharacterName.Substring(2, 2), 16), // G
                Convert.ToByte(color_CustomCharacterName.Substring(0, 2), 16)  // R
                };
            }

            //BackgroundFill
            string color_BackgroundFill = iniReader.Read("BackgroundFill", "Color").TrimStart('#');
            DatScheme.Part1.BackgroundFill_RGB = new byte[]
            {
                Convert.ToByte(color_BackgroundFill.Substring(4, 2), 16), //B
                Convert.ToByte(color_BackgroundFill.Substring(2, 2), 16), //G
                Convert.ToByte(color_BackgroundFill.Substring(0, 2), 16)  //R
            };
            DatScheme.Part1.BackgroundFill_Opacity = Convert.ToByte(iniReader.Read("BackgroundFill", "Opacity"));

            //Fonts_ID
            DatScheme.Part1.MessageWindow_MessagePart_FontID = Convert.ToUInt16(iniReader.Read("Fonts_ID", "MessageWindow_Message"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_FontID = Convert.ToUInt16(iniReader.Read("Fonts_ID", "MessageWindow_CharacterName"));
            DatScheme.Part3.ChoiceBox_Text_FontID = Convert.ToUInt16(iniReader.Read("Fonts_ID", "ChoiceBoxes"));

            //MessageWindow_LayerOrder
            DatScheme.Part1.MessageWindow_RECTOrder_SurfaceImage = Convert.ToUInt16(iniReader.Read("MessageWindow_LayerOrder", "SurfaceImage"));
            DatScheme.Part1.MessageWindow_RECTOrder_Image = Convert.ToUInt16(iniReader.Read("MessageWindow_LayerOrder", "Image"));
            DatScheme.Part1.MessageWindow_RECTOrder_SurfaceMessageText = Convert.ToUInt16(iniReader.Read("MessageWindow_LayerOrder", "SurfaceMessageText"));
            DatScheme.Part1.MessageWindow_RECTOrder_SurfaceCharacterNameText = Convert.ToUInt16(iniReader.Read("MessageWindow_LayerOrder", "SurfaceCharacterNameText"));
            DatScheme.Part1.MessageWindow_RECTOrder_MessageText = Convert.ToUInt16(iniReader.Read("MessageWindow_LayerOrder", "MessageText"));
            DatScheme.Part1.MessageWindow_RECTOrder_CharacterNameText = Convert.ToUInt16(iniReader.Read("MessageWindow_LayerOrder", "CharacterNameText"));

            //Slots_ChoiceBoxes
            DatScheme.Part1.ChoiceBox_FirstID = Convert.ToUInt16(iniReader.Read("Slots_ChoiceBoxes", "FirstID"));
            DatScheme.Part1.ChoiceBox_LastID = Convert.ToUInt16(iniReader.Read("Slots_ChoiceBoxes", "LastID"));

            //Audio
            DatScheme.Part1.AudioInstances_TotalAmount = Convert.ToUInt16(iniReader.Read("Audio", "TotalChannels"));
            DatScheme.Part1.AudioInstances_BGM = Convert.ToUInt16(iniReader.Read("Audio", "BGM_ID"));
            DatScheme.Part1.AudioInstances_Voice = Convert.ToUInt16(iniReader.Read("Audio", "Voice_ID"));
            DatScheme.Part1.AudioInstances_Unused = Convert.ToByte(iniReader.Read("Audio", "Unused_ID"));

            //MessageWindow_Message
            DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Enable = Convert.ToByte(iniReader.Read("MessageWindow_Message", "BackgroundFill_Enabled"));
            DatScheme.Part2.MessageWindow_MessagePart_Background_X = Convert.ToInt16(iniReader.Read("MessageWindow_Message", "Background_X"));
            DatScheme.Part2.MessageWindow_MessagePart_Background_Y = Convert.ToInt16(iniReader.Read("MessageWindow_Message", "Background_Y"));
            DatScheme.Part2.MessageWindow_MessagePart_Text_X = Convert.ToInt16(iniReader.Read("MessageWindow_Message", "Text_X"));
            DatScheme.Part2.MessageWindow_MessagePart_Text_Y = Convert.ToInt16(iniReader.Read("MessageWindow_Message", "Text_Y"));
            DatScheme.Part2.MessageWindow_MessagePart_Text_Width = Convert.ToUInt16(iniReader.Read("MessageWindow_Message", "Text_Width"));
            DatScheme.Part2.MessageWindow_MessagePart_Text_Height = Convert.ToUInt16(iniReader.Read("MessageWindow_Message", "Text_Height"));
            DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_X = Convert.ToInt16(iniReader.Read("MessageWindow_Message", "BackgroundFill_X"));
            DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Y = Convert.ToInt16(iniReader.Read("MessageWindow_Message", "BackgroundFill_Y"));
            DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Width = Convert.ToUInt16(iniReader.Read("MessageWindow_Message", "BackgroundFill_Width"));
            DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Height = Convert.ToUInt16(iniReader.Read("MessageWindow_Message", "BackgroundFill_Height"));
            DatScheme.Part2.MessageWindow_MessagePart_Background_ImageFile = iniReader.Read("MessageWindow_Message", "ImageFile");

            //MessageWindow_CharacterName
            DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Enable = Convert.ToByte(iniReader.Read("MessageWindow_CharacterName", "BackgroundFill_Enabled"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_Background_X = Convert.ToInt16(iniReader.Read("MessageWindow_CharacterName", "Background_X"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_Background_Y = Convert.ToInt16(iniReader.Read("MessageWindow_CharacterName", "Background_Y"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_Text_X = Convert.ToInt16(iniReader.Read("MessageWindow_CharacterName", "Text_X"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_Text_Y = Convert.ToInt16(iniReader.Read("MessageWindow_CharacterName", "Text_Y"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_Text_Width = Convert.ToUInt16(iniReader.Read("MessageWindow_CharacterName", "Text_Width"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_Text_Height = Convert.ToUInt16(iniReader.Read("MessageWindow_CharacterName", "Text_Height"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_X = Convert.ToInt16(iniReader.Read("MessageWindow_CharacterName", "BackgroundFill_X"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Y = Convert.ToInt16(iniReader.Read("MessageWindow_CharacterName", "BackgroundFill_Y"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Width = Convert.ToUInt16(iniReader.Read("MessageWindow_CharacterName", "BackgroundFill_Width"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Height = Convert.ToUInt16(iniReader.Read("MessageWindow_CharacterName", "BackgroundFill_Height"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_Unknown1 = Convert.ToUInt16(iniReader.Read("MessageWindow_CharacterName", "Unknown_1"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_Unknown2 = Convert.ToUInt16(iniReader.Read("MessageWindow_CharacterName", "Unknown_2"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_Unknown3 = Convert.ToByte(iniReader.Read("MessageWindow_CharacterName", "Unknown_3"));
            DatScheme.Part2.MessageWindow_CharacterNamePart_Background_ImageFile = iniReader.Read("MessageWindow_CharacterName", "ImageFile");

            //ChoiceBoxes
            DatScheme.Part3.ChoiceBox_Text_Alignment = Convert.ToByte(iniReader.Read("ChoiceBoxes", "TextAlignment"));
            DatScheme.Part3.ChoiceBox_BackgroundFill_Enable = Convert.ToByte(iniReader.Read("ChoiceBoxes", "BackgroundFill_Enabled"));
            DatScheme.Part3.ChoiceBox_Text_RECT_X = Convert.ToInt16(iniReader.Read("ChoiceBoxes", "RECT_X"));
            DatScheme.Part3.ChoiceBox_Text_RECT_Y = Convert.ToInt16(iniReader.Read("ChoiceBoxes", "RECT_Y"));
            DatScheme.Part3.ChoiceBox_Text_RECT_Width = Convert.ToUInt16(iniReader.Read("ChoiceBoxes", "Text_RECT_Width"));
            DatScheme.Part3.ChoiceBox_Text_RECT_Height = Convert.ToUInt16(iniReader.Read("ChoiceBoxes", "Text_RECT_Height"));
            DatScheme.Part3.ChoiceBox_Unknown1 = Convert.ToUInt16(iniReader.Read("ChoiceBoxes", "Unknown_1"));
            DatScheme.Part3.ChoiceBox_Unknown2 = Convert.ToUInt16(iniReader.Read("ChoiceBoxes", "Unknown_2"));
            DatScheme.Part3.ChoiceBox_Unknown3 = Convert.ToUInt16(iniReader.Read("ChoiceBoxes", "Unknown_3"));
            DatScheme.Part3.ChoiceBox_BackgroundFill_Height = Convert.ToUInt16(iniReader.Read("ChoiceBoxes", "BackgroundFill_Height"));
            DatScheme.Part3.ChoiceBox_BackgroundFill_Width = Convert.ToUInt16(iniReader.Read("ChoiceBoxes", "BackgroundFill_Width"));
            DatScheme.Part3.ChoiceBox_Unknown4 = Convert.ToInt16(iniReader.Read("ChoiceBoxes", "Unknown_4"));
            DatScheme.Part3.ChoiceBox_Image_YMargin = Convert.ToInt16(iniReader.Read("ChoiceBoxes", "Image_YMargin"));
            DatScheme.Part3.ChoiceBox_Image_XMargin = Convert.ToInt16(iniReader.Read("ChoiceBoxes", "Image_XMargin"));
            DatScheme.Part3.ChoiceBox_Unknown5 = Convert.ToInt16(iniReader.Read("ChoiceBoxes", "Unknown_5"));
            DatScheme.Part3.ChoiceBox_Image_NotChosen = iniReader.Read("ChoiceBoxes", "ImageFile_NotChosen");
            DatScheme.Part3.ChoiceBox_Image_NotChosen_MouseHovered = iniReader.Read("ChoiceBoxes", "ImageFile_NotChosen_MouseHovered");
            DatScheme.Part3.ChoiceBox_Image_AlreadyChosen = iniReader.Read("ChoiceBoxes", "ImageFile_AlreadyChosen");
            DatScheme.Part3.ChoiceBox_Image_AlreadyChosen_MouseHovered = iniReader.Read("ChoiceBoxes", "ImageFile_AlreadyChosen_MouseHovered");
        }

        ///<summary>Exports the DAT scheme to a string array.</summary>
        public string[] Export()
        {
            var iniCreator = new Utils.IniFile();

            ////////////
            // 1st PART
            ////////////
            
            iniCreator.Write("General", "GameIdentifier", DatScheme.Part1.GameIdentifier.ToString());
            iniCreator.Write("General", "GameSystemMode", DatScheme.Part1.GameSystemMode.ToString());
            iniCreator.Write("General", "ExclusiveFullscreen", DatScheme.Part1.ExclusiveFullscreen.ToString());
            iniCreator.Write("General", "LocaleEnglish", DatScheme.Part1.LocaleEnglish.ToString());
            iniCreator.Write("General", "WindowTitle", DatScheme.Part1.WindowTitle);
            iniCreator.Write("Security", "DATKey", DatScheme.Part1.DATKey.ToString());
            iniCreator.Write("General", "WindowWidth", DatScheme.Part1.WindowWidth.ToString());
            iniCreator.Write("General", "WindowHeight", DatScheme.Part1.WindowHeight.ToString());

            //Flags
            iniCreator.Write("Flags", "DI", DatScheme.Part1.DI_Flags.ToString());
            iniCreator.Write("Flags", "DG", DatScheme.Part1.DG_Flags.ToString());
            iniCreator.Write("Flags", "DT", DatScheme.Part1.DT_Flags.ToString());
            iniCreator.Write("Flags", "SI", DatScheme.Part1.SI_Flags.ToString());
            iniCreator.Write("Flags", "SG", DatScheme.Part1.SG_Flags.ToString());
            iniCreator.Write("Flags", "ST", DatScheme.Part1.ST_Flags.ToString());

            //Slots
            iniCreator.Write("Slots", "Objects", DatScheme.Part1.Obj_Slots.ToString());
            iniCreator.Write("Slots", "Mask", DatScheme.Part1.MSK_Slots.ToString());
            iniCreator.Write("Slots", "CGB", DatScheme.Part1.CGB_Slots.ToString());
            iniCreator.Write("Slots", "Button", DatScheme.Part1.Button_Slots.ToString());
            iniCreator.Write("Slots", "Box", DatScheme.Part1.Box_Slots.ToString());
            iniCreator.Write("Slots", "MessageLog", DatScheme.Part1.MessageLog_Slots.ToString());

            //obj IDs
            iniCreator.Write("Object_ID", "MessageWindow", DatScheme.Part1.ObjID_MessageWindow.ToString());
            iniCreator.Write("Object_ID", "MaxValueOnBoot", DatScheme.Part1.ObjID_MaxValueOnBoot.ToString());

            //Folder names
            iniCreator.Write("FolderNames", "SDT", DatScheme.Part1.SDT_FolderName);
            iniCreator.Write("FolderNames", "MG2", DatScheme.Part1.MG2_FolderName);
            iniCreator.Write("FolderNames", "MAL", DatScheme.Part1.MAL_FolderName);
            iniCreator.Write("FolderNames", "BGM", DatScheme.Part1.BGM_FolderName);
            iniCreator.Write("FolderNames", "Voice", DatScheme.Part1.Voice_FolderName);
            iniCreator.Write("FolderNames", "SE", DatScheme.Part1.SE_FolderName);
            iniCreator.Write("FolderNames", "AM2", DatScheme.Part1.AM2_FolderName);
            iniCreator.Write("FolderNames", "Unused", DatScheme.Part1.Unused_FolderName);

            //Save files
            iniCreator.Write("SaveGames", "FolderName", DatScheme.Part1.SaveData_FolderName);
            iniCreator.Write("SaveGames", "SaveName", DatScheme.Part1.SaveData_FileName);

            //Close game process
            iniCreator.Write("Process_CloseGame", "UseMessageBox", DatScheme.Part1.CloseGame_UseMessageBox.ToString());
            iniCreator.Write("Process_CloseGame", "SeenNumber", DatScheme.Part1.CloseGame_SeenNumber.ToString());
            iniCreator.Write("Process_CloseGame", "JumpLabel", DatScheme.Part1.CloseGame_JumpLabel);

            //Screen mode process
            iniCreator.Write("Process_ScreenMode", "FullScreen_SeenNumber", DatScheme.Part1.ScreenMode_ActivateFullScreen_SeenNumber.ToString());
            iniCreator.Write("Process_ScreenMode", "FullScreen_JumpLabel", DatScheme.Part1.ScreenMode_ActivateFullScreen_JumpLabel);
            iniCreator.Write("Process_ScreenMode", "Windowed_SeenNumber", DatScheme.Part1.ScreenMode_ActivateWindowed_SeenNumber.ToString());
            iniCreator.Write("Process_ScreenMode", "Windowed_JumpLabel", DatScheme.Part1.ScreenMode_ActivateWindowed_JumpLabel);

            //Fonts
            iniCreator.Write("Fonts", "Name", DatScheme.Part1.FontName);
            for (int i = 0; i < DatScheme.Part1.Font.Length; i++)
            {
                iniCreator.Write($"Fonts_{i}", "Height", DatScheme.Part1.Font[i].Height.ToString());
                iniCreator.Write($"Fonts_{i}", "Width", DatScheme.Part1.Font[i].Width.ToString());
                iniCreator.Write($"Fonts_{i}", "Weight", DatScheme.Part1.Font[i].Weight.ToString());
                iniCreator.Write($"Fonts_{i}", "DropShadow_HorizontalOffset", DatScheme.Part1.Font[i].DropShadow_HorizontalOffset.ToString());
                iniCreator.Write($"Fonts_{i}", "DropShadow_VerticalOffset", DatScheme.Part1.Font[i].DropShadow_VerticalOffset.ToString());
                iniCreator.Write($"Fonts_{i}", "DropShadow_RGBA", $"#{DatScheme.Part1.Font[i].DropShadow_RGBA[3]:X2}" +
                    $"{DatScheme.Part1.Font[i].DropShadow_RGBA[2]:X2}{DatScheme.Part1.Font[i].DropShadow_RGBA[1]:X2}" +
                    $"{DatScheme.Part1.Font[i].DropShadow_RGBA[0]:X2}");
            }

            //Message window: color customization
            iniCreator.Write("Fonts_CharacterNameCustomColor", "Enabled", DatScheme.Part1.MessageWindow_CustomColorCharName_Enable.ToString());
            for (int i = 0; i < DatScheme.Part1.Font.Length; i++)
            {
                iniCreator.Write($"Fonts_CharacterNameCustomColor_{i}", "CharacterName", DatScheme.Part1.MessageWindow_CustomColorCharName_CharName[i]);
                iniCreator.Write($"Fonts_CharacterNameCustomColor_{i}", "Color", $"#{DatScheme.Part1.MessageWindow_CustomColorCharName_RGB[i][2]:X2}" +
                    $"{DatScheme.Part1.MessageWindow_CustomColorCharName_RGB[i][1]:X2}{DatScheme.Part1.MessageWindow_CustomColorCharName_RGB[i][0]:X2}");
            }
            iniCreator.Write("Fonts", "Color_CharacterName", $"#{DatScheme.Part1.MessageWindow_CharacterName_DefaultRGB[2]:X2}" +
                    $"{DatScheme.Part1.MessageWindow_CharacterName_DefaultRGB[1]:X2}{DatScheme.Part1.MessageWindow_CharacterName_DefaultRGB[0]:X2}");
            iniCreator.Write("Fonts", "Color_MessageText", $"#{DatScheme.Part1.MessageWindow_Message_DefaultRGB[2]:X2}" +
                    $"{DatScheme.Part1.MessageWindow_Message_DefaultRGB[1]:X2}{DatScheme.Part1.MessageWindow_Message_DefaultRGB[0]:X2}");

            //Background fill
            iniCreator.Write("BackgroundFill", "Color", $"#{DatScheme.Part1.BackgroundFill_RGB[2]:X2}" +
                    $"{DatScheme.Part1.BackgroundFill_RGB[1]:X2}{DatScheme.Part1.BackgroundFill_RGB[0]:X2}");
            iniCreator.Write("BackgroundFill", "Opacity", DatScheme.Part1.BackgroundFill_Opacity.ToString());

            //Message window: font used and order of the RECTS
            iniCreator.Write("Fonts_ID", "MessageWindow_Message", DatScheme.Part1.MessageWindow_MessagePart_FontID.ToString());
            iniCreator.Write("MessageWindow_LayerOrder", "SurfaceImage", DatScheme.Part1.MessageWindow_RECTOrder_SurfaceImage.ToString());
            iniCreator.Write("MessageWindow_LayerOrder", "Image", DatScheme.Part1.MessageWindow_RECTOrder_Image.ToString());
            iniCreator.Write("MessageWindow_LayerOrder", "SurfaceMessageText", DatScheme.Part1.MessageWindow_RECTOrder_SurfaceMessageText.ToString());
            iniCreator.Write("MessageWindow_LayerOrder", "SurfaceCharacterNameText", DatScheme.Part1.MessageWindow_RECTOrder_SurfaceCharacterNameText.ToString());
            iniCreator.Write("MessageWindow_LayerOrder", "MessageText", DatScheme.Part1.MessageWindow_RECTOrder_MessageText.ToString());
            iniCreator.Write("MessageWindow_LayerOrder", "CharacterNameText", DatScheme.Part1.MessageWindow_RECTOrder_CharacterNameText.ToString());

            //Choice boxes: obj IDs slots
            iniCreator.Write("Slots_ChoiceBoxes", "FirstID", DatScheme.Part1.ChoiceBox_FirstID.ToString());
            iniCreator.Write("Slots_ChoiceBoxes", "LastID", DatScheme.Part1.ChoiceBox_LastID.ToString());

            //Audio instances
            iniCreator.Write("Audio", "TotalChannels", DatScheme.Part1.AudioInstances_TotalAmount.ToString());
            iniCreator.Write("Audio", "BGM_ID", DatScheme.Part1.AudioInstances_BGM.ToString());
            iniCreator.Write("Audio", "Voice_ID", DatScheme.Part1.AudioInstances_Voice.ToString());
            iniCreator.Write("Audio", "Unused_ID", DatScheme.Part1.AudioInstances_Unused.ToString());

            //SDT key
            iniCreator.Write("Security", "SDTKey", DatScheme.Part1.SDTKey);


            ////////////
            // 2nd PART
            ////////////

            //Message window: message part background fill and RECTs
            iniCreator.Write("MessageWindow_Message", "BackgroundFill_Enabled", DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Enable.ToString());
            iniCreator.Write("MessageWindow_Message", "Background_X", DatScheme.Part2.MessageWindow_MessagePart_Background_X.ToString());
            iniCreator.Write("MessageWindow_Message", "Background_Y", DatScheme.Part2.MessageWindow_MessagePart_Background_Y.ToString());
            iniCreator.Write("MessageWindow_Message", "Text_X", DatScheme.Part2.MessageWindow_MessagePart_Text_X.ToString());
            iniCreator.Write("MessageWindow_Message", "Text_Y", DatScheme.Part2.MessageWindow_MessagePart_Text_Y.ToString());
            iniCreator.Write("MessageWindow_Message", "Text_Width", DatScheme.Part2.MessageWindow_MessagePart_Text_Width.ToString());
            iniCreator.Write("MessageWindow_Message", "Text_Height", DatScheme.Part2.MessageWindow_MessagePart_Text_Height.ToString());
            iniCreator.Write("MessageWindow_Message", "BackgroundFill_X", DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_X.ToString());
            iniCreator.Write("MessageWindow_Message", "BackgroundFill_Y", DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Y.ToString());
            iniCreator.Write("MessageWindow_Message", "BackgroundFill_Width", DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Width.ToString());
            iniCreator.Write("MessageWindow_Message", "BackgroundFill_Height", DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Height.ToString());
            iniCreator.Write("MessageWindow_Message", "ImageFile", DatScheme.Part2.MessageWindow_MessagePart_Background_ImageFile);

            //Message window: character name part
            iniCreator.Write("Fonts_ID", "MessageWindow_CharacterName", DatScheme.Part2.MessageWindow_CharacterNamePart_FontID.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "BackgroundFill_Enabled", DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Enable.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "Background_X", DatScheme.Part2.MessageWindow_CharacterNamePart_Background_X.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "Background_Y", DatScheme.Part2.MessageWindow_CharacterNamePart_Background_Y.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "Text_X", DatScheme.Part2.MessageWindow_CharacterNamePart_Text_X.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "Text_Y", DatScheme.Part2.MessageWindow_CharacterNamePart_Text_Y.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "Text_Width", DatScheme.Part2.MessageWindow_CharacterNamePart_Text_Width.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "Text_Height", DatScheme.Part2.MessageWindow_CharacterNamePart_Text_Height.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "BackgroundFill_X", DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_X.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "BackgroundFill_Y", DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Y.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "BackgroundFill_Width", DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Width.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "BackgroundFill_Height", DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Height.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "Unknown_1", DatScheme.Part2.MessageWindow_CharacterNamePart_Unknown1.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "Unknown_2", DatScheme.Part2.MessageWindow_CharacterNamePart_Unknown2.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "Unknown_3", DatScheme.Part2.MessageWindow_CharacterNamePart_Unknown3.ToString());
            iniCreator.Write("MessageWindow_CharacterName", "ImageFile", DatScheme.Part2.MessageWindow_CharacterNamePart_Background_ImageFile);


            ////////////
            // 3rd PART
            ////////////

            //Choice boxes: main parameters
            iniCreator.Write("ChoiceBoxes", "TextAlignment", DatScheme.Part3.ChoiceBox_Text_Alignment.ToString());
            iniCreator.Write("Fonts_ID", "ChoiceBoxes", DatScheme.Part3.ChoiceBox_Text_FontID.ToString());
            iniCreator.Write("Fonts", "ChoiceBox_Text_RGB", $"#{DatScheme.Part3.ChoiceBox_Text_RGB[2]:X2}" +
                    $"{DatScheme.Part3.ChoiceBox_Text_RGB[1]:X2}{DatScheme.Part3.ChoiceBox_Text_RGB[0]:X2}");
            iniCreator.Write("ChoiceBoxes", "BackgroundFill_Enabled", DatScheme.Part3.ChoiceBox_BackgroundFill_Enable.ToString());

            //Choice boxes: RECT positions
            iniCreator.Write("ChoiceBoxes", "RECT_X", DatScheme.Part3.ChoiceBox_Text_RECT_X.ToString());
            iniCreator.Write("ChoiceBoxes", "RECT_Y", DatScheme.Part3.ChoiceBox_Text_RECT_Y.ToString());
            iniCreator.Write("ChoiceBoxes", "Text_RECT_Width", DatScheme.Part3.ChoiceBox_Text_RECT_Width.ToString());
            iniCreator.Write("ChoiceBoxes", "Text_RECT_Height", DatScheme.Part3.ChoiceBox_Text_RECT_Height.ToString());
            iniCreator.Write("ChoiceBoxes", "Unknown_1", DatScheme.Part3.ChoiceBox_Unknown1.ToString());
            iniCreator.Write("ChoiceBoxes", "Unknown_2", DatScheme.Part3.ChoiceBox_Unknown2.ToString());
            iniCreator.Write("ChoiceBoxes", "Unknown_3", DatScheme.Part3.ChoiceBox_Unknown3.ToString());
            iniCreator.Write("ChoiceBoxes", "BackgroundFill_Height", DatScheme.Part3.ChoiceBox_BackgroundFill_Height.ToString());
            iniCreator.Write("ChoiceBoxes", "BackgroundFill_Width", DatScheme.Part3.ChoiceBox_BackgroundFill_Width.ToString());
            iniCreator.Write("ChoiceBoxes", "Unknown_4", DatScheme.Part3.ChoiceBox_Unknown4.ToString());
            iniCreator.Write("ChoiceBoxes", "Image_YMargin", DatScheme.Part3.ChoiceBox_Image_YMargin.ToString());
            iniCreator.Write("ChoiceBoxes", "Image_XMargin", DatScheme.Part3.ChoiceBox_Image_XMargin.ToString());
            iniCreator.Write("ChoiceBoxes", "Unknown_5", DatScheme.Part3.ChoiceBox_Unknown5.ToString());
            iniCreator.Write("ChoiceBoxes", "ImageFile_NotChosen", DatScheme.Part3.ChoiceBox_Image_NotChosen);
            iniCreator.Write("ChoiceBoxes", "ImageFile_NotChosen_MouseHovered", DatScheme.Part3.ChoiceBox_Image_NotChosen_MouseHovered);
            iniCreator.Write("ChoiceBoxes", "ImageFile_AlreadyChosen", DatScheme.Part3.ChoiceBox_Image_AlreadyChosen);
            iniCreator.Write("ChoiceBoxes", "ImageFile_AlreadyChosen_MouseHovered", DatScheme.Part3.ChoiceBox_Image_AlreadyChosen_MouseHovered);

            return iniCreator.Create();
        }

        ///<summary>Create an original system.dat file from the DAT scheme.</summary>
        public byte[] Generate()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding shiftJIS = Encoding.GetEncoding("shift-jis");

            List<byte> outputFile = new List<byte>();

            ////////////
            // 1st PART
            ////////////

            int gameIdentifier = (int)(DatScheme.Part1.GameIdentifier - new DateTime(1970, 1, 1)).TotalSeconds;
            outputFile.AddRange(BitConverter.GetBytes(gameIdentifier));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.GameSystemMode));
            outputFile.AddRange(DatScheme.Part1.ExclusiveFullscreen);
            outputFile.AddRange(DatScheme.Part1.LocaleEnglish);
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.WindowTitle, 260, shiftJIS));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.DATKey, 260, shiftJIS));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.WindowWidth));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.WindowHeight));
            outputFile.AddRange(DatScheme.Part1.Reserved0);

            //Flags
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.DI_Flags));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.DG_Flags));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.DT_Flags));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.SI_Flags));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.SG_Flags));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.ST_Flags));

            //Slots
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.Obj_Slots));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.MSK_Slots));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.CGB_Slots));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.Button_Slots));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.Box_Slots));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.MessageLog_Slots));

            //obj IDs
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.ObjID_MessageWindow));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.ObjID_MaxValueOnBoot));

            //Folder names
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.SDT_FolderName, 260, shiftJIS));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.MG2_FolderName, 260, shiftJIS));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.MAL_FolderName, 260, shiftJIS));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.BGM_FolderName, 260, shiftJIS));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.Voice_FolderName, 260, shiftJIS));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.SE_FolderName, 260, shiftJIS));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.AM2_FolderName, 260, shiftJIS));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.Unused_FolderName, 260, shiftJIS));

            //Save files
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.SaveData_FolderName, 260, shiftJIS));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.SaveData_FileName, 260, shiftJIS));

            //Close game process
            outputFile.AddRange(DatScheme.Part1.CloseGame_UseMessageBox);
            outputFile.AddRange(DatScheme.Part1.Reserved1);
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.CloseGame_SeenNumber));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.CloseGame_JumpLabel, 260, shiftJIS));

            //Screen mode process
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.ScreenMode_ActivateFullScreen_SeenNumber));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.ScreenMode_ActivateFullScreen_JumpLabel, 260, shiftJIS));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.ScreenMode_ActivateWindowed_SeenNumber));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.ScreenMode_ActivateWindowed_JumpLabel, 260, shiftJIS));

            //Fonts
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part1.FontName, 260, shiftJIS));
            for (int i = 0; i < DatScheme.Part1.Font.Length; i++)
            {
                outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.Font[i].Height));
                outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.Font[i].Width));
                outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.Font[i].Weight));
                outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.Font[i].DropShadow_HorizontalOffset));
                outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.Font[i].DropShadow_VerticalOffset));
                outputFile.AddRange(DatScheme.Part1.Font[i].DropShadow_RGBA);
                outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.Font[i].Reserved));
            }

            //Message window: color customization
            for (int i = 0; i < DatScheme.Part1.MessageWindow_CustomColorCharName_CharName.Length; i++)
                outputFile.AddRange(
                        CreateByteArrayFromString(DatScheme.Part1.MessageWindow_CustomColorCharName_CharName[i], 50, shiftJIS));
            for (int i = 0; i < DatScheme.Part1.MessageWindow_CustomColorCharName_RGB.Length; i++)
                outputFile.AddRange(DatScheme.Part1.MessageWindow_CustomColorCharName_RGB[i]);
            outputFile.AddRange(DatScheme.Part1.MessageWindow_CharacterName_DefaultRGB);
            outputFile.AddRange(DatScheme.Part1.MessageWindow_CustomColorCharName_Enable);
            outputFile.AddRange(DatScheme.Part1.MessageWindow_Message_DefaultRGB);

            //Background fill
            outputFile.AddRange(DatScheme.Part1.BackgroundFill_RGB);
            outputFile.AddRange(DatScheme.Part1.BackgroundFill_Opacity);

            //Reserved: some of these values get copied directly to the save game file, but they are not used by the game in any way.
            outputFile.AddRange(DatScheme.Part1.ReservedSaveGame0);
            outputFile.AddRange(DatScheme.Part1.ReservedSaveGame1);
            outputFile.AddRange(DatScheme.Part1.Reserved2);
            outputFile.AddRange(DatScheme.Part1.ReservedSaveGame2);

            //Message window: font used and order of the RECTS
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.MessageWindow_MessagePart_FontID));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.MessageWindow_RECTOrder_SurfaceImage));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.MessageWindow_RECTOrder_Image));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.MessageWindow_RECTOrder_SurfaceMessageText));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.MessageWindow_RECTOrder_SurfaceCharacterNameText));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.MessageWindow_RECTOrder_MessageText));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.MessageWindow_RECTOrder_CharacterNameText));

            //Choice boxes: obj IDs slots
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.ChoiceBox_FirstID));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.ChoiceBox_LastID));

            //Audio instances
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.AudioInstances_TotalAmount));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.AudioInstances_BGM));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part1.AudioInstances_Voice));
            outputFile.AddRange(DatScheme.Part1.AudioInstances_Unused);

            //SDT key
            outputFile.AddRange(DatScheme.Part1.SDTKey_Shuffler);
            byte[] sdtkey_Trimmed = EncryptSDTKey(DatScheme.Part1.SDTKey, DatScheme.Part1.DATKey);
            byte[] sdtKey = new byte[260]; sdtKey.Initialize();
            Buffer.BlockCopy(sdtkey_Trimmed, 0, sdtKey, 0, sdtkey_Trimmed.Length);
            outputFile.AddRange(sdtKey);


            ////////////
            // 2nd PART
            ////////////

            //Message window: message part background fill and RECTs
            outputFile.AddRange(DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Enable);
            outputFile.AddRange(DatScheme.Part2.Reserved0);
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_MessagePart_Background_X));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_MessagePart_Background_Y));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_MessagePart_Text_X));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_MessagePart_Text_Y));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_MessagePart_Text_Width));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_MessagePart_Text_Height));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_X));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Y));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Width));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_MessagePart_BackgroundFill_Height));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part2.MessageWindow_MessagePart_Background_ImageFile, 260, shiftJIS));

            //Message window: character name part
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_CharacterNamePart_FontID));
            outputFile.AddRange(DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Enable);
            outputFile.AddRange(DatScheme.Part2.Reserved1);
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_CharacterNamePart_Background_X));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_CharacterNamePart_Background_Y));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_CharacterNamePart_Text_X));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_CharacterNamePart_Text_Y));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_CharacterNamePart_Text_Width));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_CharacterNamePart_Text_Height));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_X));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Y));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Width));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_CharacterNamePart_BackgroundFill_Height));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_CharacterNamePart_Unknown1));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part2.MessageWindow_CharacterNamePart_Unknown2));
            outputFile.AddRange(DatScheme.Part2.MessageWindow_CharacterNamePart_Unknown3);
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part2.MessageWindow_CharacterNamePart_Background_ImageFile, 260, shiftJIS));

            //Reserved
            outputFile.AddRange(DatScheme.Part2.Reserved2);
            outputFile.AddRange(DatScheme.Part2.Reserved3);
            outputFile.AddRange(DatScheme.Part2.Reserved4);
            outputFile.AddRange(DatScheme.Part2.Reserved5);
            outputFile.AddRange(DatScheme.Part2.Reserved6);
            outputFile.AddRange(DatScheme.Part2.Reserved7);
            outputFile.AddRange(DatScheme.Part2.Reserved8);
            outputFile.AddRange(DatScheme.Part2.Reserved9);
            outputFile.AddRange(DatScheme.Part2.Reserved10);
            outputFile.AddRange(DatScheme.Part2.Reserved11);
            outputFile.AddRange(DatScheme.Part2.Reserved12);
            outputFile.AddRange(DatScheme.Part2.Reserved13);
            outputFile.AddRange(DatScheme.Part2.Reserved14);
            outputFile.AddRange(DatScheme.Part2.Reserved15);
            outputFile.AddRange(DatScheme.Part2.Reserved16);
            outputFile.AddRange(DatScheme.Part2.Reserved17);

            while (outputFile.Count < 22916)    //Adding the padding until the next section
                outputFile.Add(0x00);

            ////////////
            // 2nd PART
            ////////////

            //Choice boxes: main parameters
            outputFile.AddRange(DatScheme.Part3.ChoiceBox_Text_Alignment);
            outputFile.AddRange(DatScheme.Part3.Reserved0);
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.ChoiceBox_Text_FontID));
            outputFile.AddRange(DatScheme.Part3.ChoiceBox_Text_RGB);
            outputFile.AddRange(DatScheme.Part3.ChoiceBox_BackgroundFill_Enable);

            //Choice boxes: RECT positions
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.ChoiceBox_Text_RECT_X));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.ChoiceBox_Text_RECT_Y));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.ChoiceBox_Text_RECT_Width));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.ChoiceBox_Text_RECT_Height));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.ChoiceBox_Unknown1));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.ChoiceBox_Unknown2));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.ChoiceBox_Unknown3));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.ChoiceBox_BackgroundFill_Height));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.ChoiceBox_BackgroundFill_Width));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.ChoiceBox_Unknown4));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.ChoiceBox_Image_YMargin));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.ChoiceBox_Image_XMargin));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.ChoiceBox_Unknown5));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part3.ChoiceBox_Image_NotChosen, 260, shiftJIS));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part3.ChoiceBox_Image_NotChosen_MouseHovered, 260, shiftJIS));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part3.ChoiceBox_Image_AlreadyChosen, 260, shiftJIS));
            outputFile.AddRange(
                CreateByteArrayFromString(DatScheme.Part3.ChoiceBox_Image_AlreadyChosen_MouseHovered, 260, shiftJIS));

            //Unused
            outputFile.AddRange(DatScheme.Part3.Reserved1);
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused0));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused1));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused2));
            outputFile.AddRange(DatScheme.Part3.Reserved2);
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused3));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused4));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused5));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused6));
            outputFile.AddRange(DatScheme.Part3.Reserved3);
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused7));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused8));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused9));
            outputFile.AddRange(DatScheme.Part3.Reserved4);
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused10));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused11));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused12));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused13));
            outputFile.AddRange(DatScheme.Part3.Reserved5);
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused14));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused15));
            outputFile.AddRange(DatScheme.Part3.Reserved6);
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused16));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused17));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused18));
            outputFile.AddRange(DatScheme.Part3.Reserved7);
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused19));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused20));
            outputFile.AddRange(DatScheme.Part3.Reserved8);
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused21));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused22));
            outputFile.AddRange(BitConverter.GetBytes(DatScheme.Part3.Unused23));

            while (outputFile.Count < 50196)    //Adding the padding until the expecting end of file
                outputFile.Add(0x00);

            return outputFile.ToArray();
        }

        ///<summary>Decrypts the SDT key.</summary>
        ///<param name="encryptedSDTKey">Byte array of the encrypted key.</param>
        ///<param name="datKey">String of the DAT key.</param>
        private static string DecryptSDTKey(byte[] encryptedSDTKey, string datKey)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding shiftJIS = Encoding.GetEncoding("shift-jis");

            //Calculate the string lengths of the SDT encrypted key and DAT key
            int datKey_Length = shiftJIS.GetByteCount(datKey);
            int sdtKey_Length = encryptedSDTKey.Length;

            //Calculate the first and last byte for the SDT key
            encryptedSDTKey[0] = unchecked((byte)(encryptedSDTKey[0] - datKey_Length));
            int lastbyte_Adjustment = 47 * (datKey_Length / 47) - datKey_Length;
            encryptedSDTKey[^1] = unchecked((byte)(encryptedSDTKey[^1] + lastbyte_Adjustment));
            
            //Nibble swap all of the SDT key bytes
            for (int i = 0; i < sdtKey_Length; i++)
                encryptedSDTKey[i] = (byte)((16 * encryptedSDTKey[i]) | (encryptedSDTKey[i] >> 4));

            //The key in reality is an ASCII string
            return Encoding.ASCII.GetString(encryptedSDTKey);
        }

        ///<summary>Encrypts the SDT key.</summary>
        ///<param name="decryptedSDTkey">String of the decrypted key.</param>
        ///<param name="datKey">String of the DAT key.</param>
        private static byte[] EncryptSDTKey(string decryptedSDTkey, string datKey)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding shiftJIS = Encoding.GetEncoding("shift-jis");

            byte[] encryptedSDTKey = Encoding.ASCII.GetBytes(decryptedSDTkey);

            //Calculate the string lengths of the SDT encrypted key and DAT key
            int datKey_Length = shiftJIS.GetByteCount(datKey);
            int sdtKey_Length = encryptedSDTKey.Length;

            //Nibble swap all of the SDT key bytes
            for (int i = 0; i < sdtKey_Length; i++)
                encryptedSDTKey[i] = (byte)((16 * encryptedSDTKey[i]) | (encryptedSDTKey[i] >> 4));

            //Calculate the first and last byte for the SDT key
            int lastbyte_Adjustment = 47 * (datKey_Length / 47) - datKey_Length;
            encryptedSDTKey[^1] = unchecked((byte)(encryptedSDTKey[^1] - lastbyte_Adjustment));
            encryptedSDTKey[0] = unchecked((byte)(encryptedSDTKey[0] + datKey_Length));

            return encryptedSDTKey;
        }

        ///<summary>Creates an arbitrary byte long array from a string,
        ///the length the game expects for all strings.</summary>
        ///<param name="name">String to include in the byte array.</param>
        ///<param name="length">Length of the resulting byte array.</param>
        ///<param name="encoding">Encoding used to store the string as.</param>
        private static byte[] CreateByteArrayFromString(string name, int length, Encoding encoding)
        {
            byte[] nameArray = new byte[length]; nameArray.Initialize();

            if (name == null)
                name = "";
            Buffer.BlockCopy(encoding.GetBytes(name), 0, nameArray, 0, encoding.GetBytes(name).Length);

            return nameArray;
        }
    }
}
