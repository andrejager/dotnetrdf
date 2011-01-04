﻿/*

Copyright Robert Vesse 2009-10
rvesse@vdesign-studios.com

------------------------------------------------------------------------

This file is part of dotNetRDF.

dotNetRDF is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

dotNetRDF is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with dotNetRDF.  If not, see <http://www.gnu.org/licenses/>.

------------------------------------------------------------------------

dotNetRDF may alternatively be used under the LGPL or MIT License

http://www.gnu.org/licenses/lgpl.html
http://www.opensource.org/licenses/mit-license.php

If these licenses are not suitable for your intended use please contact
us at the above stated email address to discuss alternative
terms.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VDS.RDF.Storage.Params
{
    /// <summary>
    /// Parameters for Store Readers and Writers which read/write using a Stream
    /// </summary>
    public class StreamParams : IStoreParams
    {
        private Stream _input = null;
        private String _filename;
        private Encoding _encoding = new UTF8Encoding(Options.UseBomForUtf8);

        /// <summary>
        /// Creates a new set of Parameters
        /// </summary>
        /// <param name="filename">File to Stream to/from</param>
        /// <remarks>If created using this constructor then a new Reader/Writer will be generated by each call to the relevant properties</remarks>
        public StreamParams(String filename)
        {
            if (filename == null) throw new ArgumentNullException("filename", "Cannot use a null Filename");
            this._filename = filename;
        }

        /// <summary>
        /// Creates a new set of Parameters
        /// </summary>
        /// <param name="filename">File to Stream to/from</param>
        /// <param name="encoding">Encoding of the File</param>
        /// <remarks>If created using this constructor then a new Reader/Writer will be generated by each call to the relevant properties</remarks>
        public StreamParams(String filename, Encoding encoding)
            : this(filename)
        {
            this._encoding = encoding;
        }

        /// <summary>
        /// Creates a new set of Parameters
        /// </summary>
        /// <param name="stream">Stream to use</param>
        /// <remarks>If created using this constructor then these Parameters are one use only</remarks>
        public StreamParams(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream", "Cannot use a null Stream");
            this._input = stream;
        }

        /// <summary>
        /// Creates a new set of Parameters
        /// </summary>
        /// <param name="stream">Stream to use</param>
        /// <param name="encoding">Encoding of the Stream</param>
        /// <remarks>If created using this constructor then these Parameters are one use only</remarks>
        public StreamParams(Stream stream, Encoding encoding)
            : this(stream)
        {
            this._encoding = encoding;
        }

        /// <summary>
        /// Gets the Stream for Reading
        /// </summary>
        public StreamReader StreamReader
        {
            get
            {
                if (this._input != null)
                {
                    return new StreamReader(this._input, this._encoding);
                }
                else
                {
                    return new StreamReader(this._filename, this._encoding);
                }
            }
        }

        /// <summary>
        /// Gets the Stream for Writing
        /// </summary>
        public StreamWriter StreamWriter
        {
            get
            {
                if (this._input != null)
                {
                    return new StreamWriter(this._input, this._encoding);
                }
                else
                {
                    return new StreamWriter(this._filename, false, this._encoding);
                }
            }
        }

        /// <summary>
        /// Gets/Sets the Encoding used for writing
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return this._encoding;
            }
            set
            {
                this._encoding = value;
            }
        }
    }
}
