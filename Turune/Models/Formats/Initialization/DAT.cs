using System;
using System.Collections.Generic;
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
            public DateTime GameIdentifier { get; set; } = DateTime.MinValue;
            public uint GameSystemMode { get; set; }
            public byte ExclusiveFullscreen { get; set; }
            public byte LocaleEnglish { get; set; }
            public string WindowTitle { get; set; } = "";
            public byte DATKey { get; set; }
            public ushort WindowWidth { get; set; }
            public ushort WindowHeight { get; set; }
            public byte[] Reserved0 { get; set; } = [0xD0, 0x02];

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
            public ushort ScreenMode_ActivateFullScreen_SeenNumber { get; set; }
            public string ScreenMode_ActivateFullScreen_JumpLabel { get; set; } = "";
            public ushort ScreenMode_ActivateWindowed_SeenNumber { get; set; }
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
            public byte[] Reserved2 { get; set; } = [0xA, 0x00];

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
            public byte[] SDTKey { get; set; } = new byte[260];

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
            public short MessageWindow_CharacterNamePart_FontID { get; set; } = 0;
            public byte MessageWindow_CharacterNamePart_BackgroundFill_Enable { get; set; } = 0;
            public byte Reserved0 { get; set; } = 0;
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
            public byte[] Reserved1 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved2 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved3 = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved4 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved5 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved6 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved7 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved8 = 
                [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved9 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved10 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved11 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved12 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved13 = [0x00, 0x00, 0x00, 0x00];
            public byte[] Reserved14 = [0x00, 0x00, 0x00, 0x00];

            //The remaining 13644 bytes are simply unassigned to any variable, so they have to be filled with 0's
        }

        //0x6A90 bytes (hardcoded value)
        internal class DATScheme_Part3
        {
            //Choice boxes: main parameters
            public byte ChoiceBox_Text_Alignment { get; set; } = 4;
            public byte Reserved0 { get; set; } = 0;
            public short ChoiceBox_Text_FontID { get; set; } = 0;
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
            public int Unused0 { get; set; } = 0;
            public int Unused1 { get; set; } = 0;
            public int Unused2 { get; set; } = 0;
            public byte[] Reserved1 { get; set; } = [0x00, 0x00, 0x00, 0x00];
            public int Unused3 { get; set; } = 0;
            public int Unused4 { get; set; } = 0;
            public int Unused5 { get; set; } = 0;
            public int Unused6 { get; set; } = 0;
            public byte[] Reserved2 =
                [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            public int Unused7 { get; set; } = 0;
            public int Unused8 { get; set; } = 0;
            public int Unused9 { get; set; } = 0;
            public byte[] Reserved3 { get; set; } = [0x00, 0x00, 0x00, 0x00];
            public int Unused10 { get; set; } = 0;
            public int Unused11 { get; set; } = 0;
            public int Unused12 { get; set; } = 0;
            public int Unused13 { get; set; } = 0;
            public byte[] Reserved4 =
                [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            public int Unused14 { get; set; } = 0;
            public int Unused15 { get; set; } = 0;
            public byte[] Reserved5 =
                [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            public int Unused16 { get; set; } = 0;
            public int Unused17 { get; set; } = 0;
            public int Unused18 { get; set; } = 0;
            public byte[] Reserved6 =
                [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            public int Unused19 { get; set; } = 0;
            public int Unused20 { get; set; } = 0;
            public byte[] Reserved7 =
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
            public int Weight { get; set; } = 44;
            public int DropShadow_HorizontalOffset { get; set; } = 1;
            public int DropShadow_VerticalOffset { get; set; } = 1;
            public byte[] DropShadow_RGBA { get; set; } = [0x41, 0x00, 0x00, 0x00]; //RGBA is a class present only in .NET 11 and above (https://learn.microsoft.com/en-us/dotnet/api/system.numerics.colors.rgba?view=net-11.0)
            public uint Reserved { get; set; }
        }
    }
}

