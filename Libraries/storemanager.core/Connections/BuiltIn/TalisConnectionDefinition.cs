﻿/*

Copyright Robert Vesse 2009-12
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
using VDS.RDF.Configuration;
using VDS.RDF.Storage;

namespace VDS.RDF.Utilities.StoreManager.Connections.BuiltIn
{
    /// <summary>
    /// Definition for connections to Talis platform stores
    /// </summary>
    public class TalisConnectionDefinition
        : BaseHttpConnectionDefinition
    {
        /// <summary>
        /// Creates a new definition
        /// </summary>
        public TalisConnectionDefinition()
            : base("Talis", "Connect to a store hosted on the Talis Platform", typeof(TalisPlatformConnector)) { }

        /// <summary>
        /// Gets/Sets the Store ID
        /// </summary>
        [Connection(DisplayName = "Store ID", DisplayOrder = 1, IsRequired = true, AllowEmptyString = false, PopulateFrom = ConfigurationLoader.PropertyStore)]
        public String StoreID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the Username
        /// </summary>
        [Connection(DisplayName = "Username", DisplayOrder = 10, IsRequired = false, AllowEmptyString = true, Type = ConnectionSettingType.String, PopulateFrom = ConfigurationLoader.PropertyUser)]
        public String Username
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the Password
        /// </summary>
        [Connection(DisplayName = "Password", DisplayOrder = 11, IsRequired = false, AllowEmptyString = true, Type = ConnectionSettingType.Password, PopulateFrom = ConfigurationLoader.PropertyPassword)]
        public String Password
        {
            get;
            set;
        }

        /// <summary>
        /// Opens the Connection
        /// </summary>
        /// <returns></returns>
        protected override IStorageProvider OpenConnectionInternal()
        {
            if (this.UseProxy)
            {
                return new TalisPlatformConnector(this.StoreID, this.Username, this.Password, this.GetProxy());
            }
            else
            {
                return new TalisPlatformConnector(this.StoreID, this.Username, this.Password);
            }
        }
    }
}