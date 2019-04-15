#region Dynamic DDL Processing
        protected void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {

            //string selectedText = ((DropDownList)sender).SelectedItem.Text;
            string selectedValue = ((DropDownList)sender).SelectedItem.Value;
            SPListItemCollection items;
            try
            {
                items = OrgUtilitiesClass.GetSPListItems((Control)sender);
            }
            catch
            {
                return;
            }

            foreach (SPListItem item in items)
            {
                string itemCode = OrgUtilitiesClass.GetItemValue(item, "Code");
                //string itemName = OrgUtilitiesClass.GetItemValue(item, "Name");
                //if ((itemCode == selectedText || itemCode == selectedValue) || (itemName == selectedText || itemName == selectedValue)) //if ddl text or value matches SP list code or name.
                if (itemCode == selectedValue)
                {
                    OrgUtilitiesClass.ProcessItem(item, this);
                    break;
                }
            }
        }
