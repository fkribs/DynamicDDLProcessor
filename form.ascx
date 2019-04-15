<asp:Panel ID="pnlRegion" runat="server">
                            <tr>
                                <td><span style="color: red">*</span>
                                    <asp:Label ID="lblRegion" runat="server" Text="Region: "></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlRegion" runat="server" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddl_SelectedIndexChanged">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:RequiredFieldValidator ID="reqRegion" runat="server" ErrorMessage="*Required" ControlToValidate="ddlRegion" Style="color: red" ValidationGroup="submit" SetFocusOnError="True"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </asp:Panel>
                        <asp:Panel ID="pnlCostCode" runat="server">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblCostCode" runat="server" Text="Cost Code"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlCostCode" runat="server" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddl_SelectedIndexChanged">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </asp:Panel>
