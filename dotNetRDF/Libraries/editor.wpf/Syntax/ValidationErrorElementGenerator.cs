﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace VDS.RDF.Utilities.Editor.Wpf.Syntax
{
    public class ValidationErrorElementGenerator
        : VisualLineElementGenerator
    {
        private WpfEditorAdaptor _adaptor;
        private VisualOptions<FontFamily, Color> _options;

        public ValidationErrorElementGenerator(WpfEditorAdaptor adaptor, VisualOptions<FontFamily, Color> options)
        {
            this._adaptor = adaptor;
            this._options = options;
        }

        public override VisualLineElement ConstructElement(int offset)
        {
            RdfParseException parseEx = this.GetException();
            if (parseEx == null) return null;
            if (parseEx.StartLine > CurrentContext.Document.LineCount) return null;
            if (this._options == null) return null;

            //Get the Start Offset which is the greater of the error start position or the offset start
            //Move it back one if it is not at start of offset/document and the error is a single point
            int startOffset = Math.Max(this.CurrentContext.Document.GetOffset(parseEx.StartLine, parseEx.StartPosition), offset);
            if (startOffset > 0 && startOffset > offset && parseEx.StartLine == parseEx.EndLine && parseEx.StartPosition == parseEx.EndPosition) startOffset--;

            //Get the End Offset which is the lesser of the error end position of the end of this line
            //If the Start and End Offsets are equal we can't show an error
            int endOffset = Math.Min(this.CurrentContext.Document.GetOffset(parseEx.EndLine, parseEx.EndPosition), this.CurrentContext.VisualLine.LastDocumentLine.EndOffset);
            if (startOffset == endOffset) return null;
            if (startOffset > endOffset) return null;

            System.Diagnostics.Debug.WriteLine("Input Offset: " + offset + " - Start Offset: " + startOffset + " - End Offset: " + endOffset);

            return new ValidationErrorLineText(this._options, this.CurrentContext.VisualLine, endOffset - startOffset);
        }

        public override int GetFirstInterestedOffset(int startOffset)
        {
            RdfParseException parseEx = this.GetException();
            if (parseEx == null) return -1;
            if (parseEx.StartLine > CurrentContext.Document.LineCount) return -1;
            if (this._options == null) return -1;

            try
            {
                int offset = CurrentContext.Document.GetOffset(parseEx.StartLine, parseEx.StartPosition);
                if (offset < startOffset)
                {
                    int endOffset = CurrentContext.Document.GetOffset(parseEx.EndLine, parseEx.EndPosition);
                    if (startOffset < endOffset)
                    {
                        return startOffset;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    if (offset > 0 && offset > (startOffset + 1) && parseEx.StartLine == parseEx.EndLine && parseEx.StartPosition == parseEx.EndPosition)
                    {
                        return offset - 1;
                    }
                    else
                    {
                        return offset;
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        private RdfParseException GetException()
        {
            if (this._adaptor.ErrorToHighlight == null) return null;
            if (this._adaptor.ErrorToHighlight is RdfParseException)
            {
                RdfParseException parseEx = (RdfParseException)this._adaptor.ErrorToHighlight;
                if (parseEx.HasPositionInformation)
                {
                    return parseEx;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
