// Copyright (c) 2009 Tom Wright. All rights reserved.
// http://rightondevelopment.blogspot.com/
// This code is distributed under the Microsoft Public License (Ms-PL).
/*
Microsoft Public License (Ms-PL)
[OSI Approved License]

This license governs use of the accompanying software. If you use the software, you
accept this license. If you do not accept the license, do not use the software.

1. Definitions
The terms "reproduce," "reproduction," "derivative works," and "distribution" have the
same meaning here as under U.S. copyright law.
A "contribution" is the original software, or any additions or changes to the software.
A "contributor" is any person that distributes its contribution under this license.
"Licensed patents" are a contributor's patent claims that read directly on its contribution.

2. Grant of Rights
(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

3. Conditions and Limitations
(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Diagnostics;
using System.ComponentModel;

namespace ZoomBoxLibrary
{

    public class ZoomBoxPanel : System.Windows.Controls.Panel, INotifyPropertyChanged
    {
        #region Public enums and constants
        public enum eZoomMode
        {
            CustomSize,
            ActualSize,
            FitWidth,
            FitHeight,
            FitPage,
            FitVisible
        }

        public enum eMouseMode
        {
            None,
            Zoom,
            Pan
        }

        public enum eWheelMode
        {
            None,
            Zoom,
            Scroll,
            Rotate
        }

        const double MINZOOM = 0.0000001;
        const double DEFAULTLEFTRIGHTMARGIN = 4;
        const double DEFAULTTOPBOTTOMMARGIN = 3;


        #endregion

        #region Dependency Properties


        // Layout
        private static DependencyProperty PaddingProperty;
        private static DependencyProperty CenterContentProperty;

        // Zoom Scale
        private static DependencyProperty MinZoomProperty;
        private static DependencyProperty MaxZoomProperty;
        private static DependencyProperty ZoomProperty;

        private static DependencyPropertyKey CanIncreaseZoomPropertyKey;
        private static DependencyPropertyKey CanDecreaseZoomPropertyKey;
        private static DependencyPropertyKey CanRotateKey;


        // Delta
        private static DependencyProperty ZoomIncrementProperty;
        private static DependencyProperty RotateIncrementProperty;
        private static DependencyProperty WheelZoomDeltaProperty;

        // modes
        private static DependencyProperty ZoomModeProperty;
        private static DependencyProperty MouseModeProperty;
        private static DependencyProperty WheelModeProperty;

        // animation
        private static DependencyProperty AnimationsProperty;
        private static DependencyProperty LockContentProperty;

        public Thickness Padding
        {
            set { SetValue(PaddingProperty, value); }
            get { return (Thickness)GetValue(PaddingProperty); }
        }
        public bool CenterContent
        {
            set { SetValue(CenterContentProperty, value); }
            get { return (bool)GetValue(CenterContentProperty); }
        }



        public double MinZoom
        {
            set { SetValue(MinZoomProperty, value); }
            get { return (double)GetValue(MinZoomProperty); }
        }
        public double MaxZoom
        {
            set { SetValue(MaxZoomProperty, value); }
            get { return (double)GetValue(MaxZoomProperty); }
        }
        public double Zoom
        {
            set { SetValue(ZoomProperty, value); }
            get { return (double)GetValue(ZoomProperty); }
        }

        public bool CanIncreaseZoom
        {
            get { return (bool)GetValue(CanIncreaseZoomPropertyKey.DependencyProperty); }
        }
        public bool CanDecreaseZoom
        {
            get { return (bool)GetValue(CanDecreaseZoomPropertyKey.DependencyProperty); }
        }
        public bool CanRotate
        {
            get { return (bool)GetValue(CanRotateKey.DependencyProperty); }
        }


        public double ZoomIncrement
        {
            set { SetValue(ZoomIncrementProperty, value); }
            get { return (double)GetValue(ZoomIncrementProperty); }
        }
        public double RotateIncrement
        {
            set { SetValue(RotateIncrementProperty, value); }
            get { return (double)GetValue(RotateIncrementProperty); }
        }

        public double WheelZoomDelta
        {
            set { SetValue(WheelZoomDeltaProperty, value); }
            get { return (double)GetValue(WheelZoomDeltaProperty); }
        }

        public ZoomBoxPanel.eWheelMode WheelMode
        {
            set { SetValue(WheelModeProperty, value); }
            get { return (ZoomBoxPanel.eWheelMode)GetValue(WheelModeProperty); }
        }

        public ZoomBoxPanel.eZoomMode ZoomMode
        {
            set { SetValue(ZoomModeProperty, value); }
            get { return (ZoomBoxPanel.eZoomMode)GetValue(ZoomModeProperty); }
        }

        public ZoomBoxPanel.eMouseMode MouseMode
        {
            set { SetValue(MouseModeProperty, value); }
            get { return (ZoomBoxPanel.eMouseMode)GetValue(MouseModeProperty); }
        }


        public bool Animations
        {
            set { SetValue(AnimationsProperty, value); }
            get { return (bool)GetValue(AnimationsProperty); }
        }
        public bool LockContent
        {
            set { SetValue(LockContentProperty, value); }
            get { return (bool)GetValue(LockContentProperty); }
        }


        #endregion

        #region Public Properties

        public bool IsZoomMode_CustomSize { set { if (value) ZoomMode = eZoomMode.CustomSize; } get { return ZoomMode == eZoomMode.CustomSize; } }
        public bool IsZoomMode_ActualSize { set { if (value) ZoomMode = eZoomMode.ActualSize; } get { return ZoomMode == eZoomMode.ActualSize; } }
        public bool IsZoomMode_FitWidth { set { if (value) ZoomMode = eZoomMode.FitWidth; } get { return ZoomMode == eZoomMode.FitWidth; } }
        public bool IsZoomMode_FitHeight { set { if (value) ZoomMode = eZoomMode.FitHeight; } get { return ZoomMode == eZoomMode.FitHeight; } }
        public bool IsZoomMode_FitPage { set { if (value) ZoomMode = eZoomMode.FitPage; } get { return ZoomMode == eZoomMode.FitPage; } }
        public bool IsZoomMode_FitVisible { set { if (value) ZoomMode = eZoomMode.FitVisible; } get { return ZoomMode == eZoomMode.FitVisible; } }

        public bool IsMouseMode_None { set { if (value) MouseMode = eMouseMode.None; } get { return MouseMode == eMouseMode.None; } }
        public bool IsMouseMode_Zoom { set { if (value) MouseMode = eMouseMode.Zoom; } get { return MouseMode == eMouseMode.Zoom; } }
        public bool IsMouseMode_Pan { set { if (value) MouseMode = eMouseMode.Pan; } get { return MouseMode == eMouseMode.Pan; } }

        public bool IsWheelMode_None { set { if (value) WheelMode = eWheelMode.None; } get { return WheelMode == eWheelMode.None; } }
        public bool IsWheelMode_Zoom { set { if (value) WheelMode = eWheelMode.Zoom; } get { return WheelMode == eWheelMode.Zoom; } }
        public bool IsWheelMode_Scroll { set { if (value) WheelMode = eWheelMode.Scroll; } get { return WheelMode == eWheelMode.Scroll; } }
        public bool IsWheelMode_Rotate { set { if (value) WheelMode = eWheelMode.Rotate; } get { return WheelMode == eWheelMode.Rotate; } }



        #endregion

        #region Member variables
        private Size childSize;

        private TranslateTransform translateTransform;
        private RotateTransform rotateTransform;
        private ScaleTransform zoomTransform;
        private TransformGroup transformGroup;

        private double panX, panY;

        private double rotateAngle, rotateCenterX, rotateCenterY;

        private int minMouseDelta = 1000000;
        private double scrollDelta = 25;
        private Point startMouseCapturePanel;
        private Cursor prevCursor = null;
        private double animationDuration = 300;


        #endregion


        #region Properties

        protected double ZoomFactor
        {
            get { return Zoom / 100; }
            set { Zoom = value * 100; }
        }

        #endregion


        #region Command Handlers
        public static RoutedUICommand rotateClockwiseCommand;
        public static RoutedUICommand RotateClockwise
        {
            get { return rotateClockwiseCommand; }
        }

        public static RoutedUICommand rotateCounterclockwiseCommand;
        public static RoutedUICommand RotateCounterclockwise
        {
            get { return rotateCounterclockwiseCommand; }
        }


        public static RoutedUICommand rotateHomeCommand;
        public static RoutedUICommand RotateHome
        {
            get { return rotateHomeCommand; }
        }
        public static RoutedUICommand rotateReverseCommand;
        public static RoutedUICommand RotateReverse
        {
            get { return rotateReverseCommand; }
        }


        private static void CanExecuteEventHandler_IfHasContent(Object sender, CanExecuteRoutedEventArgs e)
        {
            ZoomBoxPanel z = sender as ZoomBoxPanel;
            e.CanExecute = ((z != null) && (z.Children.Count > 0));
            e.Handled = true;
        }

        private static void CanExecuteEventHandler_IfCanIncreaseZoom(Object sender, CanExecuteRoutedEventArgs e)
        {
            ZoomBoxPanel z = sender as ZoomBoxPanel;
            e.CanExecute = ((z != null) && (z.CanIncreaseZoom));
            e.Handled = true;
        }
        private static void CanExecuteEventHandler_IfCanDecreaseZoom(Object sender, CanExecuteRoutedEventArgs e)
        {
            ZoomBoxPanel z = sender as ZoomBoxPanel;
            e.CanExecute = ((z != null) && (z.CanDecreaseZoom));
            e.Handled = true;
        }

        public static void ExecutedEventHandler_IncreaseZoom(Object sender, ExecutedRoutedEventArgs e)
        {
            ZoomBoxPanel z = sender as ZoomBoxPanel;
            if (z != null) z.process_ZoomCommand(true);
        }
        public static void ExecutedEventHandler_DecreaseZoom(Object sender, ExecutedRoutedEventArgs e)
        {
            ZoomBoxPanel z = sender as ZoomBoxPanel;
            if (z != null) z.process_ZoomCommand(false);
        }
        public static void ExecutedEventHandler_RotateClockwise(Object sender, ExecutedRoutedEventArgs e)
        {
            ZoomBoxPanel z = sender as ZoomBoxPanel;
            double? param = null;
            string p = e.Parameter as string;
            if ((p != null) && (p.Length > 0))
                param = Double.Parse(p);
            if (z != null) z.process_RotateCommand(true, param);
        }
        public static void ExecutedEventHandler_RotateCounterclockwise(Object sender, ExecutedRoutedEventArgs e)
        {
            ZoomBoxPanel z = sender as ZoomBoxPanel;
            double? param = null;
            string p = e.Parameter as string;
            if ((p != null) && (p.Length > 0))
                param = Double.Parse(p);
            if (z != null) z.process_RotateCommand(false, param);
        }

        public static void ExecutedEventHandler_RotateHome(Object sender, ExecutedRoutedEventArgs e)
        {
            ZoomBoxPanel z = sender as ZoomBoxPanel;
            if (z != null) z.process_RotateHomeReverse(true);
        }
        public static void ExecutedEventHandler_RotateReverse(Object sender, ExecutedRoutedEventArgs e)
        {
            ZoomBoxPanel z = sender as ZoomBoxPanel;
            if (z != null) z.process_RotateHomeReverse(false);
        }

        private static void PropertyChanged_AMode(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoomBoxPanel z = d as ZoomBoxPanel;
            if (z != null)
                z.NotifyPropertyChanged();
        }
        private static void PropertyChanged_Zoom(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoomBoxPanel z = d as ZoomBoxPanel;
            if (z != null)
                z.process_PropertyChanged_Zoom(e);
        }
        #endregion

        #region Command as methods

        public void IncreaseZoom()
        {
            process_ZoomCommand(true);
        }
        public void DecreaseZoom()
        {
            process_ZoomCommand(false);
        }

        public void DoRotateClockwise()
        {
            process_RotateCommand(true, null);
        }
        public void DoRotateClockwise(double degrees)
        {
            process_RotateCommand(true, degrees);
        }
        public void DoRotateCounterclockwise()
        {
            process_RotateCommand(false, null);
        }
        public void DoRotateCounterclockwise(double degrees)
        {
            process_RotateCommand(false, degrees);
        }
        public void DoRotateHome()
        {
            process_RotateHomeReverse(true);
        }
        public void DoRotateReverse()
        {
            process_RotateHomeReverse(false);
        }

        #endregion


        #region Constructors
        static ZoomBoxPanel()
        {
            // WPF properties
            PaddingProperty = DependencyProperty.Register(
                "Padding", typeof(Thickness), typeof(ZoomBoxPanel),
                new FrameworkPropertyMetadata(
                    new Thickness(DEFAULTLEFTRIGHTMARGIN, DEFAULTTOPBOTTOMMARGIN, DEFAULTLEFTRIGHTMARGIN, DEFAULTTOPBOTTOMMARGIN),
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Journal, null, null),
                null);

            CenterContentProperty = DependencyProperty.Register(
                "CenterContent", typeof(bool), typeof(ZoomBoxPanel),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Journal, null, null),
                null);

            MinZoomProperty = DependencyProperty.Register(
                "MinZoom", typeof(double), typeof(ZoomBoxPanel),
                new FrameworkPropertyMetadata(1.0,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Journal, PropertyChanged_Zoom, CoerceMinZoom),
                new ValidateValueCallback(ZoomBoxPanel.ValidateIsPositiveNonZero));
            MaxZoomProperty = DependencyProperty.Register(
                "MaxZoom", typeof(double), typeof(ZoomBoxPanel),
                new FrameworkPropertyMetadata(1000.0,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Journal, PropertyChanged_Zoom, CoerceMaxZoom),
                new ValidateValueCallback(ZoomBoxPanel.ValidateIsPositiveNonZero));
            ZoomProperty = DependencyProperty.Register(
                "Zoom", typeof(double), typeof(ZoomBoxPanel),
                new FrameworkPropertyMetadata(100.0,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Journal, PropertyChanged_Zoom, CoerceZoom),
                new ValidateValueCallback(ZoomBoxPanel.ValidateIsPositiveNonZero));

            CanIncreaseZoomPropertyKey = DependencyProperty.RegisterReadOnly(
                "CanIncreaseZoom", typeof(bool), typeof(ZoomBoxPanel),
                 new FrameworkPropertyMetadata(false, null, null));

            CanDecreaseZoomPropertyKey = DependencyProperty.RegisterReadOnly(
                "CanDecreaseZoom", typeof(bool), typeof(ZoomBoxPanel),
                 new FrameworkPropertyMetadata(false, null, null));

            CanRotateKey = DependencyProperty.RegisterReadOnly(
                "CanRotate", typeof(bool), typeof(ZoomBoxPanel),
                 new FrameworkPropertyMetadata(true, null, null));


            ZoomIncrementProperty = DependencyProperty.Register(
                "ZoomIncrement", typeof(double), typeof(ZoomBoxPanel),
                new FrameworkPropertyMetadata(20.0),
                new ValidateValueCallback(ZoomBoxPanel.ValidateIsPositiveNonZero));

            RotateIncrementProperty = DependencyProperty.Register(
                "RotateIncrement", typeof(double), typeof(ZoomBoxPanel),
                new FrameworkPropertyMetadata(15.0, null, CoerceRotateIncrement),
                new ValidateValueCallback(ZoomBoxPanel.ValidateIsPositiveNonZero));

            WheelZoomDeltaProperty = DependencyProperty.Register(
                "WheelZoomDelta", typeof(double), typeof(ZoomBoxPanel),
                new FrameworkPropertyMetadata(10.0),
                new ValidateValueCallback(ZoomBoxPanel.ValidateIsPositiveNonZero));

            WheelModeProperty = DependencyProperty.Register(
                "WheelMode", typeof(ZoomBoxPanel.eWheelMode), typeof(ZoomBoxPanel),
                new FrameworkPropertyMetadata(ZoomBoxPanel.eWheelMode.Zoom, PropertyChanged_AMode),
                null);

            ZoomModeProperty = DependencyProperty.Register(
                "ZoomMode", typeof(ZoomBoxPanel.eZoomMode), typeof(ZoomBoxPanel),
                new FrameworkPropertyMetadata(ZoomBoxPanel.eZoomMode.FitPage,
                 FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Journal,
                 PropertyChanged_AMode, null),
                null);

            MouseModeProperty = DependencyProperty.Register(
                "MouseMode", typeof(ZoomBoxPanel.eMouseMode), typeof(ZoomBoxPanel),
                new FrameworkPropertyMetadata(ZoomBoxPanel.eMouseMode.None, PropertyChanged_AMode),
                null);


            AnimationsProperty = DependencyProperty.Register(
                "Animations", typeof(bool), typeof(ZoomBoxPanel),
                new FrameworkPropertyMetadata(true, null),
                null);

            LockContentProperty = DependencyProperty.Register(
                "LockContent", typeof(bool), typeof(ZoomBoxPanel),
                new FrameworkPropertyMetadata(false, null),
                null);


            //-----------------------------------------------------------------
            // Commands
            CommandManager.RegisterClassCommandBinding(typeof(ZoomBoxPanel),
                new CommandBinding(NavigationCommands.IncreaseZoom,
                    ExecutedEventHandler_IncreaseZoom, CanExecuteEventHandler_IfCanIncreaseZoom));

            CommandManager.RegisterClassCommandBinding(typeof(ZoomBoxPanel),
                new CommandBinding(NavigationCommands.DecreaseZoom,
                    ExecutedEventHandler_DecreaseZoom, CanExecuteEventHandler_IfCanDecreaseZoom));

            InputGestureCollection rotateInputGestures = new InputGestureCollection();
            rotateInputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));
            rotateClockwiseCommand = new RoutedUICommand("Rotate Clockwise", "RotateClockwise", typeof(ZoomBoxPanel), rotateInputGestures);
            CommandManager.RegisterClassCommandBinding(typeof(ZoomBoxPanel),
                new CommandBinding(rotateClockwiseCommand,
                    ExecutedEventHandler_RotateClockwise, CanExecuteEventHandler_IfHasContent));

            InputGestureCollection rotateCounterInputGestures = new InputGestureCollection();
            rotateCounterInputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Alt));
            rotateCounterclockwiseCommand = new RoutedUICommand("Rotate Counterclockwise", "RotateCounterclockwise", typeof(ZoomBoxPanel), rotateCounterInputGestures);
            CommandManager.RegisterClassCommandBinding(typeof(ZoomBoxPanel),
                new CommandBinding(rotateCounterclockwiseCommand,
                    ExecutedEventHandler_RotateCounterclockwise, CanExecuteEventHandler_IfHasContent));

            InputGestureCollection rotateHomeInputGestures = new InputGestureCollection();
            rotateHomeInputGestures.Add(new KeyGesture(Key.Home, ModifierKeys.None));
            rotateHomeCommand = new RoutedUICommand("Rotate Home", "RotateHome", typeof(ZoomBoxPanel), rotateHomeInputGestures);
            CommandManager.RegisterClassCommandBinding(typeof(ZoomBoxPanel),
                new CommandBinding(rotateHomeCommand,
                    ExecutedEventHandler_RotateHome, CanExecuteEventHandler_IfHasContent));

            InputGestureCollection rotateReverseInputGestures = new InputGestureCollection();
            rotateReverseInputGestures.Add(new KeyGesture(Key.End, ModifierKeys.None));
            rotateReverseCommand = new RoutedUICommand("Rotate Reverse", "RotateReverse", typeof(ZoomBoxPanel), rotateReverseInputGestures);
            CommandManager.RegisterClassCommandBinding(typeof(ZoomBoxPanel),
                new CommandBinding(rotateReverseCommand,
                    ExecutedEventHandler_RotateReverse, CanExecuteEventHandler_IfHasContent));

            //-----------------------------------------------------------------

        }

        private static bool ValidateIsPositiveNonZero(object value)
        {
            double v = (double)value;
            return (v > 0.0) ? true : false;
        }
        private static object CoerceRotateIncrement(DependencyObject d, object value)
        {
            double dv = (double)value;
            if (dv <= 0) dv = 1;
            else if (dv > 359) dv = 359;
            return dv;
        }
        private static object CoerceZoom(DependencyObject d, object value)
        {
            double dv = (double)value;
            ZoomBoxPanel z = d as ZoomBoxPanel;
            if (z != null)
            {
                if (dv > z.MaxZoom)
                    dv = z.MaxZoom;
                else if (dv < z.MinZoom)
                    dv = z.MinZoom;
            }
            return dv;
        }
        private static object CoerceMinZoom(DependencyObject d, object value)
        {
            double dv = (double)value;
            ZoomBoxPanel z = d as ZoomBoxPanel;
            if (z != null)
            {
                if (dv <= MINZOOM)
                    dv = MINZOOM;
                if (dv > z.MaxZoom)
                    z.MaxZoom = dv;
                if (z.Zoom < dv)
                    z.Zoom = dv; ;
            }
            return dv;
        }
        private static object CoerceMaxZoom(DependencyObject d, object value)
        {
            double dv = (double)value;
            ZoomBoxPanel z = d as ZoomBoxPanel;
            if (z != null)
            {
                if (dv < z.MinZoom)
                    z.MinZoom = dv;
                if (z.Zoom > dv)
                    z.Zoom = dv; ;
            }
            return dv;
        }
        public ZoomBoxPanel()
        {
            translateTransform = new TranslateTransform();
            rotateTransform = new RotateTransform();
            zoomTransform = new ScaleTransform();

            transformGroup = new TransformGroup();

            transformGroup.Children.Add(this.rotateTransform);
            transformGroup.Children.Add(this.zoomTransform);
            transformGroup.Children.Add(this.translateTransform);

            panX = 0;
            panY = 0;
            rotateAngle = 0;
            rotateCenterX = 0;
            rotateCenterY = 0;

            childSize = new Size(1, 1);

            ClipToBounds = true;

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.MouseWheel += process_MouseWheel;

            this.MouseDown += process_MouseDown;
            this.MouseUp += process_MouseUp;
            this.MouseMove += process_MouseMove;

            this.PreviewMouseDown += process_PreviewMouseDown;
            this.PreviewMouseUp += process_PreviewMouseUp;
            this.PreviewMouseMove += process_PreviewMouseMove;


            ApplyZoom(false);

            foreach (UIElement element in base.InternalChildren)
            {
                element.RenderTransform = this.transformGroup;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Focusable = true;
        }
        #endregion


        #region Layout methods
        protected override Size MeasureOverride(Size constraint)
        {
            Size panelSize = new Size(150, 150);
            if ((!Double.IsNaN(constraint.Width)) && (!Double.IsInfinity(constraint.Width)))
                panelSize.Width = constraint.Width;
            if ((!Double.IsNaN(constraint.Height)) && (!Double.IsInfinity(constraint.Height)))
                panelSize.Height = constraint.Height;

            childSize = new Size(1, 1);
            Size infSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

            foreach (UIElement element in base.InternalChildren)
            {
                element.Measure(infSize);

                if (element.DesiredSize.Width > childSize.Width)
                    childSize.Width = element.DesiredSize.Width;

                if (element.DesiredSize.Height > childSize.Height)
                    childSize.Height = element.DesiredSize.Height;
            }

            return panelSize;
        }

        protected override Size ArrangeOverride(Size panelRect)
        {
            foreach (UIElement element in base.InternalChildren)
            {
                element.Arrange(new Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height));
            }

            RecalcPage(panelRect);

            return panelRect;
        }

        // called on panel resize, zoom mode change
        protected void RecalcPage(Size panelRect)
        {
            double desiredW = 0, desiredH = 0;
            double zoomX = 0, zoomY = 0;

            double minDimension = 5;

            switch (ZoomMode)
            {
                case eZoomMode.CustomSize:
                    break;
                case eZoomMode.ActualSize:
                    ZoomFactor = 1.0;
                    panX = CalcCenterOffset(panelRect.Width, childSize.Width, Padding.Left);
                    panY = CalcCenterOffset(panelRect.Height, childSize.Height, Padding.Top);
                    ApplyZoom(false);
                    break;

                case eZoomMode.FitWidth:
                    desiredW = panelRect.Width - Padding.Left - Padding.Right;
                    if (desiredW < minDimension) desiredW = minDimension;
                    zoomX = desiredW / childSize.Width;

                    ZoomFactor = zoomX;
                    panX = Padding.Left;
                    panY = CalcCenterOffset(panelRect.Height, childSize.Height, Padding.Top);

                    ApplyZoom(false);
                    break;
                case eZoomMode.FitHeight:
                    desiredH = panelRect.Height - Padding.Top - Padding.Bottom;
                    if (desiredH < minDimension) desiredH = minDimension;
                    zoomY = desiredH / childSize.Height;

                    ZoomFactor = zoomY;
                    panX = CalcCenterOffset(panelRect.Width, childSize.Width, Padding.Left);
                    panY = Padding.Top;

                    ApplyZoom(false);
                    break;
                case eZoomMode.FitPage:
                    desiredW = panelRect.Width - Padding.Left - Padding.Right;
                    if (desiredW < minDimension) desiredW = minDimension;
                    zoomX = desiredW / childSize.Width;
                    desiredH = panelRect.Height - Padding.Top - Padding.Bottom;
                    if (desiredH < minDimension) desiredH = minDimension;
                    zoomY = desiredH / childSize.Height;

                    if (zoomX <= zoomY)
                    {
                        ZoomFactor = zoomX;
                        panX = Padding.Left;
                        panY = CalcCenterOffset(panelRect.Height, childSize.Height, Padding.Top);
                    }
                    else
                    {
                        ZoomFactor = zoomY;
                        panX = CalcCenterOffset(panelRect.Width, childSize.Width, Padding.Left);
                        panY = Padding.Top;
                    }
                    ApplyZoom(false);
                    break;
                case eZoomMode.FitVisible:
                    desiredW = panelRect.Width - Padding.Left - Padding.Right;
                    if (desiredW < minDimension) desiredW = minDimension;
                    zoomX = desiredW / childSize.Width;
                    desiredH = panelRect.Height - Padding.Top - Padding.Bottom;
                    if (desiredH < minDimension) desiredH = minDimension;
                    zoomY = desiredH / childSize.Height;

                    if (zoomX >= zoomY)
                    {
                        ZoomFactor = zoomX;
                        panX = Padding.Left;
                        panY = CalcCenterOffset(panelRect.Height, childSize.Height, Padding.Top);
                    }
                    else
                    {
                        ZoomFactor = zoomY;
                        panX = CalcCenterOffset(panelRect.Width, childSize.Width, Padding.Left);
                        panY = Padding.Top;
                    }
                    ApplyZoom(false);
                    break;
            }
        }

        protected double CalcCenterOffset(double parent, double child, double margin)
        {
            if (CenterContent)
            {
                double offset = 0;
                offset = (parent - (child * ZoomFactor)) / 2;
                
                // if (offset > margin)
                //    return offset;

                // return offset directly to make image centered
                return offset;
            }
            return margin;
        }


        protected void ApplyZoomCommand(double delta, int factor, Point panelPoint)
        {
            if (factor > 0)
            {
                double zoom = ZoomFactor;

                for (int i = 1; i <= factor; i++)
                    zoom = zoom * delta;

                ZoomFactor = zoom;

                ApplyZoomCommand(panelPoint);
            }
        }

        protected void ApplyZoomCommand(Point panelPoint)
        {
            Point logicalPoint = transformGroup.Inverse.Transform(panelPoint);

            ZoomMode = eZoomMode.CustomSize;

            panX = -1 * (logicalPoint.X * ZoomFactor - panelPoint.X);
            panY = -1 * (logicalPoint.Y * ZoomFactor - panelPoint.Y);

            ApplyZoom(true);
        }


        protected void ApplyRotateCommand(double delta, int factor, Point panelPoint)
        {
            if (factor > 0)
            {
                rotateAngle = (rotateAngle + (delta * factor)) % 360;
                if (rotateAngle < 0)
                    rotateAngle += 360;

                ZoomMode = eZoomMode.CustomSize;

                rotateCenterX = childSize.Width / 2;
                rotateCenterY = childSize.Height / 2;

                ApplyZoom(true);
            }
        }

        protected void ApplyScrollCommand(double delta, int factor, Point panelPoint)
        {
            if (factor > 0)
            {
                double deltaY = delta * factor;

                if (ApplyPanDelta(0, deltaY))
                    ApplyZoom(true);
            }
        }

        private bool ApplyPanDelta(double deltaX, double deltaY)
        {
            double x = panX + deltaX;
            double y = panY + deltaY;

            switch (ZoomMode)
            {
                case eZoomMode.CustomSize:
                    break;
                case eZoomMode.ActualSize:
                    break;
                case eZoomMode.FitWidth:
                    // disable x move
                    x = panX;
                    break;
                case eZoomMode.FitHeight:
                    // disable x move
                    y = panY;
                    break;
                case eZoomMode.FitPage:
                    x = panX;
                    y = panY;
                    break;
                case eZoomMode.FitVisible:
                    break;
            }


            if ((x != panX) || (y != panY))
            {
                panX = x;
                panY = y;
                return true;
            }
            return false;
        }

        protected DoubleAnimation MakeZoomAnimation(double value)
        {
            DoubleAnimation ani = new DoubleAnimation(value, new Duration(TimeSpan.FromMilliseconds(animationDuration)));
            ani.FillBehavior = FillBehavior.HoldEnd;
            return ani;
        }

        protected double calcStartFromAngle(double from, double to)
        {
            if (from < to)
            {
                if (to - from > 180.0)
                    return from + 360.0;
            }
            else
            {
                if (from - to > 180.0)
                    return from - 360.0;
            }

            return from;
        }
        protected double calcAngleBetweenAngles(double from, double to)
        {
            // angle can go out of range - fix thanks to alinux08
            while (from < 0) from += 360;
            while (to < 0) to += 360;
            while (from >= 360) from -= 360;
            while (to >= 360) to -= 360;
            double a = (from <= to) ? to - from : from - to;
            if (a > 180)
                a = 360 - a;
            return a;
        }

        protected DoubleAnimation MakeRotateAnimation(double from, double to)
        {
            DoubleAnimation ani = new DoubleAnimation();
            ani.FillBehavior = FillBehavior.HoldEnd;
            ani.To = to;
            ani.From = calcStartFromAngle(from, to);
            ani.Duration = new Duration(
                TimeSpan.FromMilliseconds(animationDuration * calcAngleBetweenAngles(from, to) / 30));

            return ani;
        }

        protected void ApplyZoom(bool animate)
        {
            if ((!animate) || (!Animations))
            {
                translateTransform.BeginAnimation(TranslateTransform.XProperty, null);
                translateTransform.BeginAnimation(TranslateTransform.YProperty, null);

                zoomTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                zoomTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);

                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, null);

                translateTransform.X = panX;
                translateTransform.Y = panY;

                zoomTransform.ScaleX = ZoomFactor;
                zoomTransform.ScaleY = ZoomFactor;

                rotateTransform.Angle = rotateAngle;
                rotateTransform.CenterX = rotateCenterX;
                rotateTransform.CenterY = rotateCenterY;
            }
            else
            {
                DoubleAnimation XPropertyAnimation = MakeZoomAnimation(panX);
                DoubleAnimation YPropertyAnimation = MakeZoomAnimation(panY);
                DoubleAnimation ScaleXPropertyAnimation = MakeZoomAnimation(ZoomFactor);
                DoubleAnimation ScaleYPropertyAnimation = MakeZoomAnimation(ZoomFactor);
                DoubleAnimation AngleAnimation = MakeRotateAnimation(rotateTransform.Angle, rotateAngle);


                translateTransform.BeginAnimation(TranslateTransform.XProperty, XPropertyAnimation);
                translateTransform.BeginAnimation(TranslateTransform.YProperty, YPropertyAnimation);

                zoomTransform.BeginAnimation(ScaleTransform.ScaleXProperty, ScaleXPropertyAnimation);
                zoomTransform.BeginAnimation(ScaleTransform.ScaleYProperty, ScaleYPropertyAnimation);

                rotateTransform.CenterX = rotateCenterX;
                rotateTransform.CenterY = rotateCenterY;
                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, AngleAnimation);

            }

        }


        #endregion

        #region Event handlers
        private void process_PropertyChanged_Zoom(DependencyPropertyChangedEventArgs e)
        {
            bool canIncrease = (Zoom < MaxZoom);
            bool canDecrease = (Zoom > MinZoom);

            if (canIncrease != CanIncreaseZoom)
                this.SetValue(ZoomBoxPanel.CanIncreaseZoomPropertyKey, canIncrease);
            if (canDecrease != CanDecreaseZoom)
                this.SetValue(ZoomBoxPanel.CanDecreaseZoomPropertyKey, canDecrease);
        }

        void process_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point panelPoint = e.GetPosition(this);
            double delta = 0;

            int absDelta = Math.Abs(e.Delta);
            if ((minMouseDelta > absDelta) && (absDelta > 0))
                minMouseDelta = absDelta;
            int factor = absDelta / minMouseDelta;
            if (factor < 1)
                factor = 1;

            switch (WheelMode)
            {
                case eWheelMode.Rotate:
                    {
                        delta = RotateIncrement;
                        if (e.Delta <= 0)
                            delta *= -1;
                        ApplyRotateCommand(delta, factor, panelPoint);
                    }
                    break;

                case eWheelMode.Zoom:
                    {
                        delta = (1 + (WheelZoomDelta / 100));
                        if ((e.Delta <= 0) && (delta != 0))
                            delta = 1 / delta;
                        ApplyZoomCommand(delta, factor, panelPoint);
                    }
                    break;

                case eWheelMode.Scroll:
                    {
                        delta = scrollDelta;
                        if (e.Delta <= 0)
                            delta *= -1;
                        ApplyScrollCommand(delta, factor, panelPoint);
                    }
                    break;
            }
        }

        private void process_RotateCommand(bool clockWise, double? angle)
        {
            Point panelPoint = new Point(ActualWidth / 2, ActualHeight / 2);

            double delta = RotateIncrement;
            if (angle != null)
                delta = (double)angle;
            if (!clockWise)
                delta *= -1;
            ApplyRotateCommand(delta, 1, panelPoint);

        }

        private void process_RotateHomeReverse(bool isHome)
        {
            Point panelPoint = new Point(ActualWidth / 2, ActualHeight / 2);


            rotateAngle = isHome ? 0 : 180;

            rotateCenterX = childSize.Width / 2;
            rotateCenterY = childSize.Height / 2;

            ApplyZoom(true);

        }



        private void process_ZoomCommand(bool increase)
        {
            Point panelPoint = new Point(ActualWidth / 2, ActualHeight / 2);

            double delta = (1 + (ZoomIncrement / 100));
            if (!increase)
                delta = 1 / delta;

            ApplyZoomCommand(delta, 1, panelPoint);
        }


        void process_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (LockContent)
                e.Handled = true;

        }
        void process_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (LockContent)
                e.Handled = true;
        }
        void process_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (LockContent)
                e.Handled = true;
        }

        void process_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (MouseMode)
            {
                case eMouseMode.Pan:
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        if (((Keyboard.GetKeyStates(Key.LeftAlt) & KeyStates.Down) == 0) && ((Keyboard.GetKeyStates(Key.RightAlt) & KeyStates.Down) == 0))
                        {
                            prevCursor = this.Cursor;
                            startMouseCapturePanel = e.GetPosition(this);
                            this.CaptureMouse();
                            this.Cursor = Cursors.ScrollAll;
                        }
                    }
                    break;
            }

        }

        void process_MouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (MouseMode)
            {
                case eMouseMode.Zoom:
                    {
                        Point panelPoint = e.GetPosition(this);

                        int factor = 0;
                        double delta = (1 + (ZoomIncrement / 100));
                        switch (e.ChangedButton)
                        {
                            case MouseButton.Left:
                                factor = 1;
                                break;
                            case MouseButton.Right:
                                factor = 1;
                                delta = 1 / delta;
                                break;
                        }
                        ApplyZoomCommand(delta, factor, panelPoint);
                    }
                    break;
            }
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
                this.Cursor = prevCursor;
                prevCursor = null;
            }

        }

        void process_MouseMove(object sender, MouseEventArgs e)
        {
            Point dcurrentMousePanel = e.GetPosition(this);

            switch (MouseMode)
            {
                case eMouseMode.Zoom:
                    break;

                case eMouseMode.Pan:
                    if (this.IsMouseCaptured)
                    {
                        Point currentMousePanel = e.GetPosition(this);

                        double deltaX = currentMousePanel.X - startMouseCapturePanel.X;
                        double deltaY = currentMousePanel.Y - startMouseCapturePanel.Y;

                        if ((deltaX != 0) || (deltaY != 0))
                        {
                            startMouseCapturePanel.X = currentMousePanel.X;
                            startMouseCapturePanel.Y = currentMousePanel.Y;

                            if (ApplyPanDelta(deltaX, deltaY))
                                ApplyZoom(false);

                        }
                    }
                    break;
            }

        }


        #endregion


        #region Notifiable
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        public void NotifyPropertyChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(null));
        }
        #endregion

    }
}