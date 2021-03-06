﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PleaseWaitService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orchestra.Services
{
    using System;
    using System.Windows.Input;
    using Catel;
    using Catel.Logging;
    using Catel.Services;

    public class PleaseWaitService : IPleaseWaitService
    {
        #region Constants
        public static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        private readonly IDispatcherService _dispatcherService;

        private Cursor _previousCursor;
        #endregion

        #region Constructors
        public PleaseWaitService(IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(() => dispatcherService);

            _dispatcherService = dispatcherService;
        }
        #endregion

        public int ShowCounter { get; private set; }

        #region IPleaseWaitService Members
        public void Show(string status = "")
        {
            if (ShowCounter <= 0)
            {
                ShowCounter = 1;
            }

            UpdateStatus(status);

            _dispatcherService.BeginInvokeIfRequired(() =>
            {
                if (_previousCursor == null)
                {
                    _previousCursor = Mouse.OverrideCursor;
                }

                Mouse.OverrideCursor = Cursors.Wait;
            });
        }

        public void Show(PleaseWaitWorkDelegate workDelegate, string status = "")
        {
            Show(status);

            try
            {
                workDelegate();
            }
            finally
            {
                Hide();
            }
        }

        public void UpdateStatus(string status)
        {
            if (!string.IsNullOrWhiteSpace(status))
            {
                Log.Info(status);
            }
        }

        public void UpdateStatus(int currentItem, int totalItems, string statusFormat = "")
        {
            // not required
        }

        public void Hide()
        {
            ShowCounter = 0;

            _dispatcherService.BeginInvokeIfRequired(() =>
            {
                Mouse.OverrideCursor = _previousCursor;

                _previousCursor = null;
            });
        }

        public void Push(string status = "")
        {
            if (ShowCounter == 0)
            {
                Show(status);
            }
            else
            {
                ShowCounter++;
            }
        }

        public void Pop()
        {
            ShowCounter--;

            if (ShowCounter <= 0)
            {
                Hide();
            }
        }
        #endregion
    }
}