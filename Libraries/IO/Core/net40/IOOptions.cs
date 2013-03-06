﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF.Parsing.Tokens;
using VDS.RDF.Writing;

namespace VDS.RDF
{
    public static class IOOptions
    {
        private static int _defaultCompressionLevel = WriterCompressionLevel.More;
        private static TokenQueueMode _defaultTokenQueueMode = TokenQueueMode.SynchronousBufferDuringParsing;
        private static bool _utf8Bom = false;
        private static bool _useDTDs = true;
        private static bool _multiThreadedWriting = false;
        private static bool _uriLoaderCaching = true;
        private static int _uriLoaderTimeout = 15000;
        private static bool _forceBlockingIO = false;

        /// <summary>
        /// Gets/Sets the Default Compression Level used for Writers returned by the <see cref="MimeTypesHelper">MimeTypesHelper</see> class when the writers implement <see cref="ICompressingWriter">ICompressingWriter</see>
        /// </summary>
        public static int DefaultCompressionLevel
        {
            get
            {
                return _defaultCompressionLevel;
            }
            set
            {
                _defaultCompressionLevel = value;
            }
        }

#if !NO_URICACHE

        /// <summary>
        /// Gets/Sets whether the <see cref="UriLoader">UriLoader</see> uses caching
        /// </summary>
        public static bool UriLoaderCaching
        {
            get
            {
                return _uriLoaderCaching;
            }
            set
            {
                _uriLoaderCaching = value;
            }
        }
#endif

        /// <summary>
        /// Gets/Sets the Timeout for URI Loader requests (Defaults to 15 seconds)
        /// </summary>
        public static int UriLoaderTimeout
        {
            get
            {
                return _uriLoaderTimeout;
            }
            set
            {
                if (value > 0)
                {
                    _uriLoaderTimeout = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets whether a UTF-8 BOM is used for UTF-8 Streams created by dotNetRDF (this does not affect Streams passed directly to methods as open streams cannot have their encoding changed)
        /// </summary>
        public static bool UseBomForUtf8
        {
            get
            {
                return _utf8Bom;
            }
            set
            {
                _utf8Bom = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether Blocking IO should be forced
        /// </summary>
        /// <remarks>
        /// Blocking IO refers to how the parsing sub-system reads in inputs, it will use Blocking/Non-Blocking IO depending on the input source.  In most cases the detection of which to use should never cause an issue but theoretically in some rare cases using non-blocking IO may lead to incorrect parsing errors being thrown (premature end of input detected), if you suspect this is the case try enabling this setting.  If you still experience this problem with this setting enabled then there is some other issue with your input.
        /// </remarks>
        public static bool ForceBlockingIO
        {
            get
            {
                return _forceBlockingIO;
            }
            set
            {
                _forceBlockingIO = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether a DTD should be used for some XML formats to compress output
        /// </summary>
        public static bool UseDtd
        {
            get
            {
                return _useDTDs;
            }
            set
            {
                _useDTDs = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether multi-theaded writing is permitted
        /// </summary>
        /// <remarks>
        /// In some contexts multi-threaded writing may not even work due to restrictions on thread types since we use the <see cref="System.Threading.WaitAll">WaitAll()</see> method which is only valid in <strong>MTA</strong> contexts.
        /// </remarks>
        public static bool AllowMultiThreadedWriting
        {
            get
            {
                return _multiThreadedWriting;
            }
            set
            {
                _multiThreadedWriting = value;
            }
        }

        /// <summary>
        /// Gets/Sets the default token queue mode used for tokeniser based parsers
        /// </summary>
        public static TokenQueueMode DefaultTokenQueueMode
        {
            get
            {
                return _defaultTokenQueueMode;
            }
            set
            {
                _defaultTokenQueueMode = value;
            }
        }
    }
}
