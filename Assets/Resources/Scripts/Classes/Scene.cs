using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene
{
    public const int TUTORIAL = -2;

        public const int TUT_FIRST_TIME = 0;
        public const int TUT_RETURN_TIMES = 1;

    public const int MAIN_MENU = -1;

        public const int MM_BUTTONS_DELAYED = 0;
        public const int MM_BUTTONS_NO_DELAY = 1;

    public const int WAITING_ROOM = 0;

        public const int WR_DEFAULT = 0;

    public const int GP1_ROLES = 1;

        public const int GP1_DEFAULT = 0;

    public const int GP2_TEXTENTRY = 2;

        public const int GP2_PRE_SUBMIT = 0;
        public const int GP2_POST_SUBMIT = 1;

    public const int GP3_CHOOSEWHOSETURN = 3;

        public const int GP3_TICKER_START = 0;
        public const int GP3_PRE_READOUT = 1;
        public const int GP3_READOUT = 2;

    public const int GP4_PLAY = 4;

        public const int GP4_IN_PROGRESS = 0;
        public const int GP4_ROUND_END = 1;

    public const int GP5_REVEALSINTRO = 5;

        public const int GP5_READY_DISALLOWED = 0;
        public const int GP5_READY_ALLOWED = 1;

    public const int GP6_ENDGAME = 6;

        // PAGER PAGES = 0, 1, 2, ...

                public const int GP6_PAGE_PRE_REVEAL = 0;
                public const int GP6_PAGE_POST_REVEAL = 1;
                public const int GP6_PAGE_READIED_REVEAL = 2;

        public const int GP6_PAGER_END = -1;

                public const int GP6_END_RESULTS_REVEALED = 0;
                public const int GP6_END_ALL_UNLOCKED = 1;
                public const int GP6_END_NEW_ROUND_READY = 2;

    // ALL FRAGMENTS

        // ALL SECONDARY STATES

            public const int NO_TERTIARY_STATE = -1;

}
