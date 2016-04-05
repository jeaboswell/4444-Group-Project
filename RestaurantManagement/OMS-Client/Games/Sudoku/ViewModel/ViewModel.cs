//
// Copyright (c) 2014 Han Hung
// 
// This program is free software; it is distributed under the terms
// of the GNU General Public License v3 as published by the Free
// Software Foundation.
//
// http://www.gnu.org/licenses/gpl-3.0.html
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

using OMS.Games.Sudoku.Model;
using OMS.Games.Sudoku.Model.Enums;
using OMS.Games.Sudoku.Model.Structures;
using OMS.Games.Sudoku.View;
using OMS.Games.Sudoku.ViewModel.CustomEventArgs;
using OMS.Games.Sudoku.ViewModel.Enums;
using OMS.Games.Sudoku.ViewModel.GameGenerator;

namespace OMS.Games.Sudoku.ViewModel
{
    public class ViewModelClass : INotifyPropertyChanged
    {
        #region . Variables, Constants, And other Declarations .

        #region . Variables .

        private static object _lock = new object();
        private static ViewModelClass _instance;


        private GameTimer _timer;
        private GamesManager _games;
        private string _statusMsg;
        private StartButtonStateEnum _startButtonState;
        private GameModel _model;
        private tableInterface _view;
        private DifficultyLevels _gameLevel;
        private bool _isEnableGameControls;
        private bool _isShowGameGrid;
        private bool _isEnterNotes;
        private bool _isShowSolution;
        private bool _isShowNotes;
        private string _gameTimeElapsed;

        #endregion

        #region . Other Declarations .

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #endregion

        #region . Constructors .

        // Declared private so no one else can.
        private ViewModelClass()
        { }

        #endregion

        #region . Properties .

        #region . Properties: Public Read-only .

        /// <summary>
        /// Gets the elapsed time from the timer class.
        /// </summary>
        public string ElapsedTime
        {
            get
            {
                if (GameInProgress)                         // Is the game in progress
                {
                    if (_timer == null)                     // Yes, is the timer variable null?
                        return "";                          // Yes, then return a blank string
                    return _timer.ElapsedTime;              // No, return the timer's elapsed time
                }
                else
                    return "";                              // No game, so return a blank string
            }
            private set
            {
                OnPropertyChanged();                        // If set, raise the property change flag
            }
        }

        /// <summary>
        /// Gets the formated string to display in the Game Completed window.
        /// </summary>
        public string GameTimeElapsed
        {
            get
            {
                return _gameTimeElapsed;
            }
            private set
            {
                _gameTimeElapsed = string.Format("Your time is {0}.", value);
            }
        }

        /// <summary>
        /// Gets the status message to display on the form.
        /// </summary>
        public string StatusMessage
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_statusMsg))      // Is the status message variable null or empty?
                    return "";                                  // Yes, return a blank string
                return _statusMsg;                              // No, return the string.
            }
            private set
            {
                _statusMsg = value;                             // Save the value
                OnPropertyChanged();                            // Raise the property change flag
            }
        }

        /// <summary>
        /// Gets the start button state.
        /// </summary>
        public StartButtonStateEnum StartButtonState
        {
            get
            {
                return _startButtonState;
            }
            private set
            {
                _startButtonState = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the game count for the Very Easy level.
        /// </summary>
        public string GameCountVeryEasy
        {
            get
            {
                return GetGameCount(DifficultyLevels.VeryEasy);
            }
            private set
            {
                OnPropertyChanged("GameCountVeryEasy");
            }
        }

        /// <summary>
        /// Gets the game count for the Easy level.
        /// </summary>
        public string GameCountEasy
        {
            get
            {
                return GetGameCount(DifficultyLevels.Easy);
            }
            private set
            {
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the game count for the Medium level.
        /// </summary>
        public string GameCountMedium
        {
            get
            {
                return GetGameCount(DifficultyLevels.Medium);
            }
            private set
            {
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the game count for the Hard level.
        /// </summary>
        public string GameCountHard
        {
            get
            {
                return GetGameCount(DifficultyLevels.Hard);
            }
            private set
            {
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the game count for the Expert level.
        /// </summary>
        public string GameCountExpert
        {
            get
            {
                return GetGameCount(DifficultyLevels.Expert);
            }
            private set
            {
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the flag indicating whether the game related controls should be enabled or not.
        /// </summary>
        public bool IsEnableGameControls
        {
            get
            {
                return _isEnableGameControls;
            }
            private set
            {
                _isEnableGameControls = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the flag indicating whether the game grid should be show or not.
        /// </summary>
        public bool IsShowGameGrid
        {
            get
            {
                return _isShowGameGrid;
            }
            private set
            {
                _isShowGameGrid = value;
                OnPropertyChanged();
            }
        }

        #region . Properties: Cell Contents .

        // Property pointers to individual cells of the puzzle.

        #region . Properties: Row 1 Cells .
        public CellClass Cell00
        {
            get
            {
                return _model[0, 0];
            }
        }
        public CellClass Cell10
        {
            get
            {
                return _model[1, 0];
            }
        }
        public CellClass Cell20
        {
            get
            {
                return _model[2, 0];
            }
        }
        public CellClass Cell30
        {
            get
            {
                return _model[3, 0];
            }
        }
        public CellClass Cell40
        {
            get
            {
                return _model[4, 0];
            }
        }
        public CellClass Cell50
        {
            get
            {
                return _model[5, 0];
            }
        }
        public CellClass Cell60
        {
            get
            {
                return _model[6, 0];
            }
        }
        public CellClass Cell70
        {
            get
            {
                return _model[7, 0];
            }
        }
        public CellClass Cell80
        {
            get
            {
                return _model[8, 0];
            }
        }

        #endregion

        #region . Properties: Row 2 Cells .
        public CellClass Cell01
        {
            get
            {
                return _model[0, 1];
            }
        }

        public CellClass Cell11
        {
            get
            {
                return _model[1, 1];
            }
        }

        public CellClass Cell21
        {
            get
            {
                return _model[2, 1];
            }
        }

        public CellClass Cell31
        {
            get
            {
                return _model[3, 1];
            }
        }

        public CellClass Cell41
        {
            get
            {
                return _model[4, 1];
            }
        }

        public CellClass Cell51
        {
            get
            {
                return _model[5, 1];
            }
        }

        public CellClass Cell61
        {
            get
            {
                return _model[6, 1];
            }
        }

        public CellClass Cell71
        {
            get
            {
                return _model[7, 1];
            }
        }

        public CellClass Cell81
        {
            get
            {
                return _model[8, 1];
            }
        }

        #endregion

        #region . Properties: Row 3 Cells .
        public CellClass Cell02
        {
            get
            {
                return _model[0, 2];
            }
        }

        public CellClass Cell12
        {
            get
            {
                return _model[1, 2];
            }
        }

        public CellClass Cell22
        {
            get
            {
                return _model[2, 2];
            }
        }

        public CellClass Cell32
        {
            get
            {
                return _model[3, 2];
            }
        }

        public CellClass Cell42
        {
            get
            {
                return _model[4, 2];
            }
        }

        public CellClass Cell52
        {
            get
            {
                return _model[5, 2];
            }
        }

        public CellClass Cell62
        {
            get
            {
                return _model[6, 2];
            }
        }

        public CellClass Cell72
        {
            get
            {
                return _model[7, 2];
            }
        }

        public CellClass Cell82
        {
            get
            {
                return _model[8, 2];
            }
        }

        #endregion

        #region . Properties: Row 4 Cells .
        public CellClass Cell03
        {
            get
            {
                return _model[0, 3];
            }
        }

        public CellClass Cell13
        {
            get
            {
                return _model[1, 3];
            }
        }

        public CellClass Cell23
        {
            get
            {
                return _model[2, 3];
            }
        }

        public CellClass Cell33
        {
            get
            {
                return _model[3, 3];
            }
        }

        public CellClass Cell43
        {
            get
            {
                return _model[4, 3];
            }
        }

        public CellClass Cell53
        {
            get
            {
                return _model[5, 3];
            }
        }

        public CellClass Cell63
        {
            get
            {
                return _model[6, 3];
            }
        }

        public CellClass Cell73
        {
            get
            {
                return _model[7, 3];
            }
        }

        public CellClass Cell83
        {
            get
            {
                return _model[8, 3];
            }
        }

        #endregion

        #region . Properties: Row 5 Cells .
        public CellClass Cell04
        {
            get
            {
                return _model[0, 4];
            }
        }

        public CellClass Cell14
        {
            get
            {
                return _model[1, 4];
            }
        }

        public CellClass Cell24
        {
            get
            {
                return _model[2, 4];
            }
        }

        public CellClass Cell34
        {
            get
            {
                return _model[3, 4];
            }
        }

        public CellClass Cell44
        {
            get
            {
                return _model[4, 4];
            }
        }

        public CellClass Cell54
        {
            get
            {
                return _model[5, 4];
            }
        }

        public CellClass Cell64
        {
            get
            {
                return _model[6, 4];
            }
        }

        public CellClass Cell74
        {
            get
            {
                return _model[7, 4];
            }
        }

        public CellClass Cell84
        {
            get
            {
                return _model[8, 4];
            }
        }

        #endregion

        #region . Properties: Row 6 Cells .
        public CellClass Cell05
        {
            get
            {
                return _model[0, 5];
            }
        }

        public CellClass Cell15
        {
            get
            {
                return _model[1, 5];
            }
        }

        public CellClass Cell25
        {
            get
            {
                return _model[2, 5];
            }
        }

        public CellClass Cell35
        {
            get
            {
                return _model[3, 5];
            }
        }

        public CellClass Cell45
        {
            get
            {
                return _model[4, 5];
            }
        }

        public CellClass Cell55
        {
            get
            {
                return _model[5, 5];
            }
        }

        public CellClass Cell65
        {
            get
            {
                return _model[6, 5];
            }
        }

        public CellClass Cell75
        {
            get
            {
                return _model[7, 5];
            }
        }

        public CellClass Cell85
        {
            get
            {
                return _model[8, 5];
            }
        }

        #endregion

        #region . Properties: Row 7 Cells .
        public CellClass Cell06
        {
            get
            {
                return _model[0, 6];
            }
        }

        public CellClass Cell16
        {
            get
            {
                return _model[1, 6];
            }
        }

        public CellClass Cell26
        {
            get
            {
                return _model[2, 6];
            }
        }

        public CellClass Cell36
        {
            get
            {
                return _model[3, 6];
            }
        }

        public CellClass Cell46
        {
            get
            {
                return _model[4, 6];
            }
        }

        public CellClass Cell56
        {
            get
            {
                return _model[5, 6];
            }
        }

        public CellClass Cell66
        {
            get
            {
                return _model[6, 6];
            }
        }

        public CellClass Cell76
        {
            get
            {
                return _model[7, 6];
            }
        }

        public CellClass Cell86
        {
            get
            {
                return _model[8, 6];
            }
        }

        #endregion

        #region . Properties: Row 8 Cells .
        public CellClass Cell07
        {
            get
            {
                return _model[0, 7];
            }
        }

        public CellClass Cell17
        {
            get
            {
                return _model[1, 7];
            }
        }

        public CellClass Cell27
        {
            get
            {
                return _model[2, 7];
            }
        }

        public CellClass Cell37
        {
            get
            {
                return _model[3, 7];
            }
        }

        public CellClass Cell47
        {
            get
            {
                return _model[4, 7];
            }
        }

        public CellClass Cell57
        {
            get
            {
                return _model[5, 7];
            }
        }

        public CellClass Cell67
        {
            get
            {
                return _model[6, 7];
            }
        }

        public CellClass Cell77
        {
            get
            {
                return _model[7, 7];
            }
        }

        public CellClass Cell87
        {
            get
            {
                return _model[8, 7];
            }
        }

        #endregion

        #region . Properties: Row 9 Cells .
        public CellClass Cell08
        {
            get
            {
                return _model[0, 8];
            }
        }

        public CellClass Cell18
        {
            get
            {
                return _model[1, 8];
            }
        }

        public CellClass Cell28
        {
            get
            {
                return _model[2, 8];
            }
        }

        public CellClass Cell38
        {
            get
            {
                return _model[3, 8];
            }
        }

        public CellClass Cell48
        {
            get
            {
                return _model[4, 8];
            }
        }

        public CellClass Cell58
        {
            get
            {
                return _model[5, 8];
            }
        }

        public CellClass Cell68
        {
            get
            {
                return _model[6, 8];
            }
        }

        public CellClass Cell78
        {
            get
            {
                return _model[7, 8];
            }
        }

        public CellClass Cell88
        {
            get
            {
                return _model[8, 8];
            }
        }

        #endregion

        #endregion

        #endregion

        #region . Properties: Public Read/Write .

        /// <summary>
        /// Gets or sets the Game Level.
        /// </summary>
        public DifficultyLevels GameLevel
        {
            get
            {
                return _gameLevel;
            }
            set
            {
                bool bLoadNewGame = (_gameLevel != value);                      // Save whether the value changed or not
                _gameLevel = value;                                             // Save the value to our local variable
                Properties.Settings.Default.Level = _gameLevel.GetHashCode();   // Save the value to the application settings
                if (bLoadNewGame)                                               // Did the value change from earlier?
                    LoadNewGame();                                              // Yes, then load a new game
                OnPropertyChanged();                                            // Raise the property changed flag
            }
        }

        /// <summary>
        /// Gets or sets the checkbox indicating whether the user wants to enter notes or not.
        /// </summary>
        public bool IsEnterNotes
        {
            get
            {
                return _isEnterNotes;
            }
            set
            {
                _isEnterNotes = value;                                      // Save the value
                OnPropertyChanged();                                        // Raise the property changed flag
            }
        }

        public bool IsShowNotes
        {
            get
            {
                return _isShowNotes;
            }
            set
            {
                _isShowNotes = value;                                       // Save the value
                if (IsValidGame())                                          // Is there a valid game?
                    if (value)                                              // Yes, does user want to show notes?
                        _model.ShowNotes();                                 // Yes, show the notes
                    else
                        _model.HideNotes();                                 // No, hide the notes
                OnPropertyChanged();                                        // Raise the property changed flag
            }
        }

        public bool IsShowSolution
        {
            get
            {
                return _isShowSolution;
            }
            set
            {
                _isShowSolution = value;                                    // Save the value
                if (IsValidGame())                                          // Is there a valid game?
                    if (value)                                              // Yes, does the user want to show solution?
                        _model.ShowSolution();                              // Yes, show the solution
                    else
                        _model.HideSolution(IsShowNotes);                   // No, hide the solution
                OnPropertyChanged();                                        // Raise the property changed flag
            }
        }

        #endregion

        #region . Properties: Private .

        private bool GameInProgress { get; set; }
        private bool PuzzleComplete { get; set; }

        #endregion

        #endregion

        #region . Event Handlers: Other Events .

        private void GameTimerEventHandler(object sender, GameTimerEventArgs e)
        {
            ElapsedTime = e.ElapsedTime;                    // Save the elapsed time string from the timer class
        }

        private void GamesManagerEventHandler(object sender, GameManagerEventArgs e)
        {
            switch (e.Level)                                    // Which level raised the event
            {
                case DifficultyLevels.VeryEasy:                 // Very Easy level raised event
                    GameCountVeryEasy = e.Count.ToString();     // Save the count to the Very Easy level property
                    break;

                case DifficultyLevels.Easy:                     // Easy level raised the event
                    GameCountEasy = e.Count.ToString();         // Save the count to the Easy level property
                    break;

                case DifficultyLevels.Medium:                   // Medium level raised the event
                    GameCountMedium = e.Count.ToString();       // Save the count to the Medium level property
                    break;

                case DifficultyLevels.Hard:                     // Hard level raised the event
                    GameCountHard = e.Count.ToString();         // Save the count to the Hard level property
                    break;

                case DifficultyLevels.Expert:                   // Expert level raised the event
                    GameCountExpert = e.Count.ToString();       // Save the count to the Expert level property
                    break;
            }
        }

        #endregion

        #region . Methods .

        #region . Methods: Public, Static, Read-only .

        /// <summary>
        /// Gets an instance of this class.
        /// </summary>
        /// <param name="window">View object that called this instance.</param>
        /// <returns></returns>
        public static ViewModelClass GetInstance(tableInterface window)
        {
            if (_instance == null)                          // Is the instance variable null?
            {
                lock (_lock)                                // Yes, obtain the object lock
                {
                    if (_instance == null)                  // Check again if the instance variable is null
                    {
                        _instance = new ViewModelClass();   // It is.  So instantiate it
                        _instance.InitClass(window);        // Initialize it
                    }
                }
            }
            return _instance;                               // Return a pointer to the instance
        }

        #endregion

        #region . Methods: View Form event methods .

        /// <summary>
        /// Process the Close button click.
        /// </summary>
        internal bool CloseClicked()
        {
            return CloseClick();
        }

        /// <summary>
        /// Process the New button click.
        /// </summary>
        internal void NewClicked()
        {
            LoadNewGame();
        }

        /// <summary>
        /// Process the Start button click.
        /// </summary>
        internal void StartClicked()
        {
            StartGame();
        }

        /// <summary>
        /// Process the Reset button click.
        /// </summary>
        internal void ResetClicked()
        {
            ResetGame();
        }

        /// <summary>
        /// Process the About button click.
        /// </summary>
        internal void AboutClicked()
        {
            ShowAboutBox();
        }

        /// <summary>
        /// Process the Print button click.
        /// </summary>
        internal void PrintClicked()
        {
            // TODO: Implement puzzle print routines.
        }

        /// <summary>
        /// Process the cell click event on the specified column and row.
        /// </summary>
        /// <param name="col">Column of the cell where the click event happened.</param>
        /// <param name="row">Row of the cell where the click event happened.</param>
        internal void CellClicked(Int32 col, Int32 row)
        {
            ProcessCellClick(col, row);
        }

        #endregion

        #region . Methods: Private .

        private void InitClass(tableInterface window)
        {
            Debug.WriteLine("Initialize View Model ...");
            _view = window;                                         // Save the window
            _timer = new GameTimer();                               // Instantiate a new timer class
            _timer.GameTimerEvent += GameTimerEventHandler;         // Set the timer event handler
            _games = new GamesManager();                            // Instantiate a new game manager class
            _games.GamesManagerEvent += GamesManagerEventHandler;   // Set the event handler
            PuzzleComplete = false;                                 // Clear some flags
            GameInProgress = false;
            StartButtonState = StartButtonStateEnum.Start;        // Set the initial start button state to disabled
            EnableGameControls(false, false);                       // Disable the game controls and hide the grid
            _model = new GameModel(null);                           // Initialize the model with null
            LoadSettings();                                         // Load settings
        }

        private void EnableGameControls(bool bEnable, bool bShow)
        {
            IsEnableGameControls = bEnable;
            IsShowGameGrid = bShow;
        }

        private void LoadSettings()
        {
            GameLevel = ConvertGameLevel(Properties.Settings.Default.Level);    // Load game level from settings
            // TODO: Load previous game if any.
            // If there was a previous game saved, then ask player
            // if they want to play old game or load a new game.
        }

        private bool CloseClick()
        {
            if (GameInProgress)                                     // Any game in progress?
            {                                                       // Yes, ask the user what they want to do
                MessageBoxResult iResults = MessageBox.Show("There is a game in progress.  Are you sure you want to quit?", "Sudoku", MessageBoxButton.YesNo);
                if (iResults == MessageBoxResult.Yes)               // User really wants to quit?
                    StopGame();                                     // Yes, stop the game
                else
                    return true;                                    // No, user doesn't want to quit so return True
            }
            else
                StopGame();                                         // No game in progress, stop the game
            return false;                                           // Return false
        }

        private void StopGame()
        {
            _games.StopGamesManager();                              // Stop the game manager background tasks
            SaveSettings();                                         // Save the settings to the config file
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.Level = (int)GameLevel;     // Save the game level settings to the application settings
            Properties.Settings.Default.Save();                     // Save to the config file
        }

        private void LoadNewGame()
        {
            if (GameInProgress)                                     // Is there already a game in progress?
            {                                                       // Yes, ask the user if they want to play a new game
                MessageBoxResult iResults = MessageBox.Show("There is already a game in progress.  Do you want to play a new game?", "Sudoku", MessageBoxButton.YesNo);
                if (iResults == MessageBoxResult.Yes)               // Did user say yes?
                {
                    GameEnded(false);                               // Then stop the current game
                    GetNewGame();                                   // Load a new game
                }
            }
            else
            {                                                       // No game in progress
                EnableGameControls(false, false);                   // Disable the game controls and hide the grid
                StatusMessage = "";                                 // Clear the status box
                ElapsedTime = "";                                   // Clear the elapsed time display
                GetNewGame();                                       // Load a new game
            }
        }

        private void GameEnded(bool bShowDialog)
        {
            _timer.StopTimer();                                     // Stop the timer
            PuzzleComplete = true;                                  // Raise the puzzle complete flag
            GameInProgress = false;                                 // Clear the game in progress flag
            ElapsedTime = "";                                       // Clear the elapsed time display
            if (bShowDialog)                                        // Do we show the puzzle complete dialog?
            {
                EnableGameControls(false, true);                    // Yes, disable the game controls, but show the game grid
                StartButtonState = StartButtonStateEnum.Start;		// Disable the start button
                StatusMessage = "Puzzle complete!";                 // Set the status message
                GameTimeElapsed = _timer.ElapsedTime;               // Save the elapsed time
                _view.ShowGameCompletedDialog();                    // Show the puzzle complete dialog
            }
            else
            {                                                       // No, don't display the puzzle complete dialog
                EnableGameControls(false, false);                   // Disable the game controls and hide the game grid
                StartButtonState = StartButtonStateEnum.Start;      // Set the start button state to "Start"
            }
        }

        private void GetNewGame()
        {
            if (_games.GameCount(GameLevel) > 0)                    // Are there games available? 
            {
                _model = new GameModel(_games.GetGame(GameLevel));  // Yes, load a game in the model class
                StartButtonState = StartButtonStateEnum.Start;      // Set the start button state to Start
                StatusMessage = "New game loaded.  Click 'Start Game' button to begin.";
            }
            else                                                    // No games available, tell user.
                StatusMessage = "No games available for the selected level.  Please select another level.";
        }

        private void StartGame()
        {
            if (GameInProgress)                                     // Is there a game in progress?
                if (StartButtonState == StartButtonStateEnum.Pause) // Yes, set the start button state to Pause
                    PauseGame();                                    // Pause the game
                else
                    ResumeGame();                                   // No, must be resume state, so resume the game
            else
                StartNewGame();                                     // No game, so start it
        }

        private void PauseGame()
        {
            StartButtonState = StartButtonStateEnum.Resume;         // Set start button state to Resume
            _timer.PauseTimer();                                    // Pause the timer
            EnableGameControls(false, false);                       // Disable the game controls and hide the grid
        }

        private void ResumeGame()
        {
            StartButtonState = StartButtonStateEnum.Pause;          // Set the start button state to Pause
            _timer.ResumeTimer();                                   // Resume the timer
            EnableGameControls(true, true);                         // Show the game controls and show the grid
        }

        private void StartNewGame()
        {
            GameInProgress = true;                                  // Raise the game in progress flag
            ShowBoard();                                            // Show the game 
            _timer.StartTimer();                                    // Start the game timer
            StartButtonState = StartButtonStateEnum.Pause;          // Set the start button state to Pause
            EnableGameControls(true, true);                         // Enable the game controls and show the grid
            UpdateEmptyCount();                                     // Display the number of empty cells
        }

        private void ShowBoard()
        {
            ClearForm();                                            // Clear the form
            UpdateAllCells();                                       // Force all cells to update
            PuzzleComplete = false;                                 // Clear the puzzle complete flag
        }

        private void UpdateEmptyCount(Int32 count = 0)
        {
            if (IsValidGame())                                      // Is there a valid game?
            {
                _model.EmptyCount += count;                         // Add the count
                StatusMessage = string.Format("{0} empty cells out of 81.", _model.EmptyCount); // Display the count
                if (_model.GameComplete)                            // Is the game done?
                    GameEnded(true);                                // Yes, run the game ended routine
            }
        }

        private bool IsValidGame()
        {   // Return true if the there is a game in progress and the model is valid
            return (GameInProgress && (_model != null) && (_model.CellList != null));
        }

        private void ClearForm()
        {
            ClearCheckboxes();                                      // Clear the check boxes
            StatusMessage = "";                                     // Clear the status message
        }

        private void ClearCheckboxes()
        {
            IsEnterNotes = false;                                   // Set the properties to false
            IsShowNotes = false;
            IsShowSolution = false;
        }

        private void UpdateAllCells()
        {
            if (IsValidGame())                                      // Is there a valid game?
                foreach (CellClass item in _model.CellList)         // Yes, loop through the list of cells
                    OnPropertyChanged(item.CellName);               // Raise the property change
        }

        private void ResetGame()
        {
            if (IsValidGame())                                      // Is there a valid game?
            {
                ClearForm();                                        // Yes, clear the form
                _timer.ResetTimer();                                // Reset the timer
                _model.ResetPuzzle();                               // Reset the puzzle
                UpdateEmptyCount();                                 // Update the empty count status
            }
        }

        private void ShowAboutBox()
        {
            //_view.ShowAboutBox();                                   // Show the About box dialog
        }

        private void ProcessCellClick(Int32 col, Int32 row)
        {
            if (IsValidGame() && (_model[col, row].CellState != CellStateEnum.Answer))// Is a game in progress and the cell is in the answer state
                ProcessNumberPad(col, row, _view.ShowNumberPad());  // Yes, display the number pad and process it
        }

        private void ProcessNumberPad(Int32 col, Int32 row, InputPadStateEnum value)
        {
            switch (value)
            {
                case InputPadStateEnum.HintRaised:                  // User clicked Hint button
                    ProcessHint(col, row);                          // Show the hint for the specified cell
                    break;

                case InputPadStateEnum.ClearRaised:                 // User clicked Clear button
                    ProcessClearCell(col, row);                     // Clear the specified cell
                    break;

                case InputPadStateEnum.Number1:                     // User clicked the "1" button
                    ProcessNumberPad(col, row, 1);                  // Process the number
                    break;

                case InputPadStateEnum.Number2:                     // User clicked the "2" button
                    ProcessNumberPad(col, row, 2);                  // Process the number
                    break;

                case InputPadStateEnum.Number3:                     // User clicked the "3" button
                    ProcessNumberPad(col, row, 3);                  // Process the number
                    break;

                case InputPadStateEnum.Number4:                     // User clicked the "4" button
                    ProcessNumberPad(col, row, 4);                  // Process the number
                    break;

                case InputPadStateEnum.Number5:                     // User clicked the "5" button
                    ProcessNumberPad(col, row, 5);                  // Process the number
                    break;

                case InputPadStateEnum.Number6:                     // User clicked the "6" button
                    ProcessNumberPad(col, row, 6);                  // Process the number
                    break;

                case InputPadStateEnum.Number7:                     // User clicked the "7" button
                    ProcessNumberPad(col, row, 7);                  // Process the number
                    break;

                case InputPadStateEnum.Number8:                     // User clicked the "8" button
                    ProcessNumberPad(col, row, 8);                  // Process the number
                    break;

                case InputPadStateEnum.Number9:                     // User clicked the "9" button
                    ProcessNumberPad(col, row, 9);                  // Process the number
                    break;
            }
        }

        private void ProcessHint(Int32 col, Int32 row)
        {
            _model[col, row].CellState = CellStateEnum.Hint;        // Set the cell state to Hint
            UpdateEmptyCount(-1);                                   // Decrement the empty count
        }

        private void ProcessClearCell(Int32 col, Int32 row)
        {
            if (_model[col, row].CellState != CellStateEnum.Answer) // Is cell state Answer?
            {                                                       // No, then process it
                if (UndoEmptyCount(_model[col, row]))               // Do we need to undo the empty count?
                    UpdateEmptyCount(1);                            // Yes, then increment the empty count
                _model[col, row].CellState = CellStateEnum.Blank;   // Set the cell state to blank
                _model[col, row].UserAnswer = 0;                    // Clear out the user's answer
                _model.ComputeNote(col, row);                       // Recompute notes
            }
        }

        private bool UndoEmptyCount(CellClass cell)
        {   // Return true if the cell state is hint or the user's answer is correct
            return ((cell.CellState == CellStateEnum.Hint) || (cell.CellState == CellStateEnum.UserInputCorrect));
        }

        private void ProcessNumberPad(Int32 col, Int32 row, Int32 value)
        {
            if (IsEnterNotes)                                         // Is the Enter Notes checkbox checked?
                ProcessNotes(col, row, value);                      // Yes, then process notes
            else
                ProcessAnswer(col, row, value);                     // No, then process user's answer
        }

        private void ProcessNotes(Int32 col, Int32 row, Int32 value)
        {
            if (Common.IsValidAnswer(value))                        // Is the value a valid answer?
                ProcessNotes(_model[col, row], value - 1);          // Yes, the process it
        }

        private void ProcessNotes(CellClass cell, Int32 index)
        {
            switch (cell.CellState)                                     // What is the cell state?
            {
                case CellStateEnum.Notes:                               // Already showing notes?
                    cell.Notes[index].State = !cell.Notes[index].State; // Yes, then toggle the state of the specified note
                    break;

                case CellStateEnum.Blank:                               // Blank?
                    cell.ClearNotes();                                  // Clear all notes
                    cell.CellState = CellStateEnum.Notes;               // Set the state to notes
                    cell.Notes[index].State = !cell.Notes[index].State; // Toggle the state of the specified note
                    break;
            }
        }

        private void ProcessAnswer(Int32 col, Int32 row, Int32 value)
        {
            _model[col, row].UserAnswer = value;                                // Save user's answer
            if (_model[col, row].CellState == CellStateEnum.UserInputCorrect)   // Is it correct?
            {
                ScanNotes(col, row, value);                                     // Yes, turn off notes 
                UpdateEmptyCount(-1);                                           // Update the empty count
            }
        }

        private void ScanNotes(Int32 col, Int32 row, Int32 value)
        {
            for (Int32 i = 0; i < 9; i++)                                       // Loop through the column and row
            {
                CheckNotes(_model[col, i], value - 1);                          // Scan the column
                CheckNotes(_model[i, row], value - 1);                          // Scan the row
            }
            List<CellClass> uList = _model.RegionCells(_model[col, row].Region);    // Retrieve the list of cells in the region
            if (uList != null)                                                      // List valid?
                for (Int32 i = 0; i < 9; i++)                                       // Loop through the region
                    if (uList[i] != null)                                           // If it's not null
                        CheckNotes(uList[i], value - 1);                            // Scan the region
        }

        private void CheckNotes(CellClass cell, Int32 index)
        {   // If the cell state is Notes and the specified note state is set
            if ((cell.CellState == CellStateEnum.Notes) && (cell.Notes[index].State))
                cell.Notes[index].State = false;                            // Clear the note state
        }

        private static DifficultyLevels ConvertGameLevel(Int32 value)
        {
            try
            {
                if (Enum.IsDefined(typeof(DifficultyLevels), value))    // Is the value part of the enum?
                    return (DifficultyLevels)value;                     // Yes, return the value
            }
            catch (Exception)
            {
                // TODO: What to do here?
                // Maybe log error to Application.Event log?
            }
            return DifficultyLevels.VeryEasy;                           // No, default to Very Easy level
        }

        private string GetGameCount(DifficultyLevels level)
        {
            if (_games == null)                                         // Game manager class instantiated?
                return "0";                                             // No, then just return zero
            return _games.GameCount(level).ToString();                  // Yes, then return the actual game count
        }

        #endregion

        #region . Interface Implementation .

        // This routine is normally called from the Set accessor of each property
        // that is bound to the a WPF control.  We use the [CallMemberName] attribute
        // so that the property name of the caller will be substituted as the argument.
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion
    }
}
