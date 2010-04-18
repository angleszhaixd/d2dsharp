﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Managed.Graphics.Direct2D;
using Managed.Graphics.DirectWrite;
using Managed.Graphics.Imaging;
using Managed.Graphics.Forms;

namespace Managed.D2DSharp.SimpleText
{
    public partial class ClientDrawingEffectsControl : Direct2DControl
    {
        private static string _text = "Hello World using   DirectWrite!";
        private static float _dpiScaleX;
        private static float _dpiScaleY;

        private TextFormat _textFormat;
        private TextLayout _textLayout;
        private SolidColorBrush _blackBrush;
        private ColorDrawingEffect _redColorDrawingEffect = new ColorDrawingEffect(Color.FromARGB(Colors.Red, 1));
        private CustomTextRendererWithEffects _customRenderer;

        static ClientDrawingEffectsControl()
        {
            DirectWriteFactory.GetDpiScale(out _dpiScaleX, out _dpiScaleY);
        }

        public ClientDrawingEffectsControl()
        {
            InitializeComponent();
        }

        protected override void OnCreateDeviceIndependentResources(Direct2DFactory factory)
        {
            base.OnCreateDeviceIndependentResources(factory);

            this._textFormat = DirectWriteFactory.CreateTextFormat("Magneto",
                null,
                FontWeight.Normal,
                FontStyle.Normal,
                FontStretch.Normal,
                72,
                "en-us");

            this._textFormat.TextAlignment = TextAlignment.Center;
            this._textFormat.ParagraphAlignment = ParagraphAlignment.Center;

            float width = ClientSize.Width / _dpiScaleX;
            float height = ClientSize.Height / _dpiScaleY;

            this._textLayout = DirectWriteFactory.CreateTextLayout(
                _text,
                this._textFormat,
                width,
                height);

            this._textLayout.SetFontSize(100, new TextRange(20, 6));
            this._textLayout.SetDrawingEffect(_redColorDrawingEffect, new TextRange(20, 6));

            TextRange textRange;
            ClientDrawingEffect effect = this._textLayout.GetDrawingEffect(21, out textRange);

            this._textLayout.SetUnderline(true, new TextRange(20, 11));
            this._textLayout.SetFontWeight(FontWeight.Bold, new TextRange(20, 11));

            using (Typography typography = DirectWriteFactory.CreateTypography())
            {
                typography.AddFontFeature(FontFeatureTag.StylisticSet7, 1);
                this._textLayout.SetTypography(typography, new TextRange(0, _text.Length));
            }
        }

        protected override void OnCleanUpDeviceIndependentResources()
        {
            base.OnCleanUpDeviceIndependentResources();
            this._textFormat.Dispose();
            this._textLayout.Dispose();
        }

        protected override void OnCreateDeviceResources(WindowRenderTarget renderTarget)
        {
            base.OnCreateDeviceResources(renderTarget);

            this._blackBrush = renderTarget.CreateSolidColorBrush(Color.FromARGB(Colors.Black, 1));
            this._customRenderer = new CustomTextRendererWithEffects(this.Direct2DFactory, renderTarget, this._blackBrush);
        }

        protected override void OnCleanUpDeviceResources()
        {
            base.OnCleanUpDeviceResources();
            this._blackBrush.Dispose();
        }

        protected override void OnRender(WindowRenderTarget renderTarget)
        {
            PointF origin = new PointF(0, 0);
            this._textLayout.Draw(this._customRenderer, origin.X, origin.Y);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this._textLayout != null)
            {
                this._textLayout.MaxWidth = ClientSize.Width / _dpiScaleX;
                this._textLayout.MaxHeight = ClientSize.Height / _dpiScaleY;
            }
        }
    }
}