 #region Dynamic DDL Processing Helper
        /// <summary>
        /// Uses SPListItems to toggle control visibilities and bind drop-down-lists:
        /// Checks SPListItem for fields (column names) containing keywords "Show" and "Available".
        /// SharePoint field names directly after the keywords are parsed and processed according to the keyword:
        /// "Show" finds panels that correspond to the above-mentioned field name and apply boolean value of SPListItem field value to panel's visibility property.
        /// "Available" finds ddls that correspond to the above-mentioned field name and binds its source to the SPList that corresponds to keyword field's name.
        ///
        /// ex: ddlGLAccount's SelectedIndexChanged uses OrgUtilitiesClass.GetSPListItems((Control)sender) to get the SPListItemCollection corresponding to it's name (AP GL Accounts)
        ///     SelectedIndexChanged then finds the SPListItem in that collection that corresponds to the sender's SelectedItem and passes it to ProcessItem() with an instance of it's parent control
        ///     ProcessItem() finds "AP Show Sub Codes" column in SPListItem and uses it's boolean value to hide/show pnlSubCodes
        ///     ProcessItem() finds "Available Sub Code" column in SPListItem and uses it's split string value (ex: T.C,T.D) to populate ddlSubCode's datasource
        /// (Use ucManualPayment.ddl_SelectedIndexChanged() for usage example)
        /// </summary>
        /// <param name="pItem">SPListItem to be searched for keyword fields/columns.</param>
        /// <param name="pFormInstance">Control whose children to toggle visibility and bind sources to.</param>
        public static void ProcessItem(SPListItem pItem, Control pFormInstance) 
        {
            SPFieldCollection fields = pItem.Fields;

            foreach (SPField field in fields)
            {
                SPFormattedField ff = new SPFormattedField(field);
                if (ff.ContainsShow) //field contains the "Show" keyword
                {
                    //process control visibility
                    foreach (Panel pnl in pFormInstance.FindDescendants<Panel>())
                    {
                        string controlName = OrgUtilitiesClass.GetFormattedControlName(pnl);
                        if (!ff.FormattedName.Contains(controlName)) continue; //skip processing if the field name doesn't correspond to the current control name
                        if (ff.isBool)
                        {
                            bool showItem;
                            try
                            {
                                showItem = Convert.ToBoolean(GetItemValue(pItem, ff.FieldName));
                            }
                            catch(System.FormatException)
                            {
                                showItem = true;
                            } 
                            pnl.Visible = (bool)showItem;
                        }
                        else throw new Exception(String.Format("ucManualPayment.ProcessItem(): {0} field '{1}' is not Boolean type.", pItem.ToString(), ff.FieldName));
                    }
                }

                if (ff.ContainsAvailable)//field contains the "Available keyword"
                {
                    //process ddl datasource
                    foreach (DropDownList ddl in pFormInstance.FindDescendants<DropDownList>())
                    {
                        //get list items
                        string controlName = OrgUtilitiesClass.GetFormattedControlName(ddl);
                        if (!ff.FormattedName.Contains(controlName)) continue; //skip processing if the field name doesn't correspond to the current control name
                        string listTitle = ff.FieldName.Split(new string[] { " " }, 2, StringSplitOptions.None)[1]; //removes keyword from field name (i.e. "Available")
                        var codes = pItem[ff.FieldName]; //get values of available field
                        SPListItemCollection allListItems;
                        try
                        {
                            allListItems = OrgUtilitiesClass.GetSPListItems(listTitle); //find list items that corresponds to field name
                        }
                        catch
                        {
                            continue;
                        }
                        //populate datasource with list items
                        List<GenericSPItem> availableListItems = new List<GenericSPItem> { new GenericSPItem { Code = "", Name = "" } }; //blank inserted at beginning
                        if (!(codes == null || codes.ToString() == "ALL"))
                        {
                            foreach (SPListItem item in allListItems)
                            {
                                foreach (string code in codes.ToString().Split(','))
                                {
                                    if (code == GetItemValue(item, "Code"))
                                    {
                                        string name = GetItemValue(item,"Name");
                                        availableListItems.Add(new GenericSPItem { Code = code, Name = name });
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (SPListItem item in allListItems)
                            {
                                string code = GetItemValue(item, "Code");
                                string name = GetItemValue(item,"Name");
                                availableListItems.Add(new GenericSPItem { Code = code, Name = name });
                            }
                        }
                        //set ddl data source
                        ddl.Items.Clear();
                        ddl.DataSource = null;
                        //remove binding fields to accomodate rare edge cases where binding already occurred
                        ddl.DataValueField = "Code";
                        ddl.DataTextField = "Name";
                        ddl.DataSource = availableListItems;
                        ddl.DataBind();
                    }
                }
            }
        }
        public static string GetItemValue(SPItem item, string column)
        {
            string result = string.Empty;
            try
            {
                result = item[column].ToString();
                if (result.Contains(";#"))
                {
                    result = result.Split('#')[1]; //fixes strange bug where field values prepend the datatype, ex: string;#Intangible Completion
                }
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return result.Trim();
        }
      

        public static List<GenericSPItem> GetGenericSPItemsDeserialized(String listName)
        {
            SPListItemCollection items = GetSPListItemsByListName(listName);

            List<GenericSPItem> result = new List<GenericSPItem>();
            foreach (SPListItem item in items)
            {
                if (item == null)
                {
                    continue;
                }
                string title = item["Name"].ToString();
                GenericSPItem spItem = new GenericSPItem
                {
                    Name = (item["Name"] ?? "").ToString(),
                    Code = (item["Code"] ?? "").ToString(),
                };
                if (spItem.Name.Contains(";#"))
                {
                    spItem.Name = spItem.Name.Split('#')[1]; //fixes strange bug where field values prepend the datatype, ex: string;#Intangible Completion
                }
                result.Add(spItem);
            }
            return result;
        }
        public static List<GenericSPItem> GetGenericSPItemsDeserialized(String listName, string pFilterField, string pFilterValue)
        {
            SPListItemCollection items = GetSPListItemsByListName(listName, pFilterField, pFilterValue);

            List<GenericSPItem> result = new List<GenericSPItem>();
            foreach (SPListItem item in items)
            {
                if (item == null)
                {
                    continue;
                }
                string title = item["Name"].ToString();
                GenericSPItem spItem = new GenericSPItem
                {
                    Name = (item["Name"] ?? "").ToString(),
                    Code = (item["Code"] ?? "").ToString(),
                };
                if (spItem.Name.Contains(";#"))
                {
                    spItem.Name = spItem.Name.Split('#')[1]; //fixes strange bug where field values prepend the datatype, ex: string;#Intangible Completion
                }
                result.Add(spItem);
            }
            return result;
        }

        public static SPListItemCollection GetSPListItems(Control pControl)
        {
            string requestedListTitle = GetFormattedControlName(pControl);
            return GetSPListItems(requestedListTitle);
        }

        public static SPListItemCollection GetSPListItems(string pRequestedListTitle)
        {
            string requestedListTitle = pRequestedListTitle.Replace(" ", "").ToUpperInvariant();
            SPWeb web = SPContext.Current.Web;
            List<SPList> lists = new List<SPList>();
            foreach (SPList list in web.Lists)
            {
                string formattedListTitle = list.Title.Replace(" ", "").ToUpperInvariant();
                if (formattedListTitle.EndsWith(requestedListTitle) || formattedListTitle.Substring(0, formattedListTitle.Length - 1).EndsWith(requestedListTitle)) //second condition accounts for plurals.
                {
                    lists.Add(list);
                }
            }
            if (lists.Count == 0)
            {
                throw new Exception("OrgUtilitiesClass.GetSPListItems(): List name does not correspond to SP list.");
            }
            if (lists.Count > 1)
            {
                throw new Exception("OrgUtilitiesClass.GetSPListItems(): Ambiguous list name. List name corresponds to multiple SP lists.");
            }
            return lists[0].Items;
        }

        public static SPListItemCollection GetSPListItemsByListName(string pListName)
        {
            SPQuery camlQuery = new SPQuery();
            camlQuery.Query = "<OrderBy><FieldRef Name='Sort_x0020_Order' Ascending='True' /></OrderBy>";
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists[pListName];
            return list.GetItems(camlQuery);
        }
        public static SPListItemCollection GetSPListItemsByListName(string pListName, string pFilterField, string pFilterValue)
        {
            SPQuery camlQuery = new SPQuery();
            camlQuery.Query = String.Format("@<Where><Contains><FieldRef Name='{0}' /><Value Type='Text'>{1}</Value></Contains></Where><OrderBy><FieldRef Name='Sort_x0020_Order' Ascending='True' /></OrderBy>", pFilterField, pFilterValue);
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists[pListName];
            return list.GetItems(camlQuery);
        }

        public static SPFieldCollection GetSPFieldsByControl(Control pControl)
        {
            SPWeb web = SPContext.Current.Web;
            List<SPList> lists = new List<SPList>();
            foreach (SPList list in web.Lists)
            {
                string controlName = GetFormattedControlName(pControl);
                string formattedListTitle = list.Title.Replace(" ", "").ToUpperInvariant();
                if (formattedListTitle.EndsWith(controlName) || formattedListTitle.Substring(0, formattedListTitle.Length - 1).EndsWith(controlName)) //second condition accounts for plurals.
                {
                    lists.Add(list);
                }
            }
            if (lists.Count == 0)
            {
                throw new Exception("OrgUtilitiesClass.GetSPListItemsByControl(): Control does not correspond to SP list.");
            }
            if (lists.Count > 1)
            {
                throw new Exception("OrgUtilitiesClass.GetSPListItemsByControl(): Ambiguous control name. Control name corresponds to multiple SP lists.");
            }
            return lists[0].Fields;
        }

        public static SPFieldCollection GetSPFieldsByListName(string pListName)
        {
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists[pListName];
            return list.Fields;
        }
        public static IEnumerable<TControl> FindDescendants<TControl>(this Control parent) where TControl : Control
        {//Extension method taken from https://stackoverflow.com/questions/7362482/get-all-web-controls-of-a-specific-type-on-a-page
            if (parent == null) throw new ArgumentNullException("control");

            if (parent.HasControls())
            {
                foreach (Control childControl in parent.Controls)
                {
                    var candidate = childControl as TControl;
                    if (candidate != null) yield return candidate;

                    foreach (var nextLevel in FindDescendants<TControl>(childControl))
                    {
                        yield return nextLevel;
                    }
                }
            }
        }

        public static string GetFormattedControlName(Control pControl, int pPrefixLength = 3)//assumes 3 character prefix convention (lbl, 
        {
            string controlName = pControl.ClientID;
            controlName = controlName.Substring(controlName.LastIndexOf('_') + 1);//Org_ucManualPayment_ddlSubCode -> ddlSubCode
            controlName = controlName.Substring(pPrefixLength);//ddlSubCode -> SubCode
            controlName = controlName.ToUpperInvariant();//SubCode -> SUBCODE
            return controlName;
        }

        public static List<SPAPCostCode> GetCostCodesByCode(List<string> pCodes = null)
        {
            List<SPAPCostCode> codes = OrgUtilitiesClass.GetSPAPCostCodeItemsDeserialized();
            if (pCodes != null)
            {
                codes = codes.Where(item =>
                    pCodes.Any(code => item.Code == code)).ToList();
            }
            codes.Insert(0, new SPAPCostCode());
            return codes;
        }
        #endregion
