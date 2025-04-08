<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Kosciuszko.aspx.cs" Inherits="Polozenie_rysunkow_baza.Kosciuszko" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="style.css" rel="stylesheet" />
    <title>Położenie rysunków</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="RegistrationPanel" runat="server">
            <asp:ScriptManager ID="scriptManagerTK" runat="server" />
            <div class="container box">
                <div class="registration">
                    <h2>Podaj imię i nazwisko</h2>
                    <%--<asp:Label ID="UsernameLabel" AssociatedControlID="UsernameTextBox" />--%>
                    <asp:TextBox ID="UsernameTextBox" CssClass="text" ForeColor="blue" ToolTip="Imię Nazwisko, np. Jan Kowalski" runat="server" />
                    <asp:Button ID="RegisterButton" runat="server" Text="Zarejestruj" OnClick="RegisterButton_Click" />
                    <asp:RegularExpressionValidator ID="UsernameTextBoxValidator" runat="server" ControlToValidate="UsernameTextBox" ErrorMessage="Identyfikator nie jest zgodny ze wzorcem" ValidationExpression="^[A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]{2,} [A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]{2,}$" ForeColor="Red"></asp:RegularExpressionValidator>
                    <asp:RequiredFieldValidator ID="UsernameTextBoxRequiredValidator" runat="server" ControlToValidate="UsernameTextBox" ErrorMessage="Nie wprowadzono żadnej wartości" ForeColor="Red"></asp:RequiredFieldValidator>
                    <asp:Label ID="ErrorLabelTK" runat="server" ForeColor="Red" />
                </div>
            </div>
        </asp:Panel>
        <div id="main-container">
            <div class="container">
                <asp:Panel ID="UserPanel" runat="server" Visible="false" CssClass="userpanel">
                    <h2><asp:Label ID="UsernameTK" runat="server" CssClass="text" ForeColor="red"></asp:Label></h2>
                    <asp:Button ID="ButtonResetTK" runat="server" Text="Zeruj Dane" OnClick="ButtonResetTK_Click" />
                    <asp:DetailsView ID="DetailsLocalViewTK" runat="server" CssClass="details-view" AutoGenerateRows="False">
                        <Fields>
                            <asp:BoundField DataField="DataUruchomienia" HeaderText="Data Uruchomienia" ItemStyle-CssClass="field" />
                            <asp:BoundField DataField="GodzinaUruchomienia" HeaderText="Godzina Uruchomienia" ItemStyle-CssClass="field" />
                            <asp:BoundField DataField="IleNastapiloZmianRozmiaruZdjecia" HeaderText="Ile Zmian Rozmiaru" ItemStyle-CssClass="field" />
                            <asp:BoundField DataField="IleNastapiloZmianPolozeniaZdjecia" HeaderText="Ile Zmian Położenia" ItemStyle-CssClass="field" />
                            <asp:BoundField DataField="MinimalnyCzasReakcji" HeaderText="Czas Minimalny" ItemStyle-CssClass="field" />
                            <asp:BoundField DataField="MaksymalnyCzasReakcji" HeaderText="Czas Maksymalny" ItemStyle-CssClass="field" />
                        </Fields>
                    </asp:DetailsView>
                    <asp:UpdatePanel ID="UpdatePanelUsers" runat="server">
                        <ContentTemplate>
                            <asp:DropDownList ID="UsersDropDownTK" runat="server" AutoPostBack="true" OnSelectedIndexChanged="UsersDropDownTK_SelectedIndexChanged" CssClass="text" ForeColor="green"></asp:DropDownList>
                            <asp:Label ID="NoUsersLabel" runat="server" Text="Brak użytkowników" Visible="false" ForeColor="red" />
                            <asp:Timer ID="TimerUsers" runat="server" Interval="15000" OnTick="TimerUsers_Tick"></asp:Timer>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:DetailsView ID="DetailsViewTK" runat="server" CssClass="details-view" Visible="false" AutoGenerateRows="False">
                        <Fields>
                            <asp:BoundField DataField="DataUruchomienia" HeaderText="Data Uruchomienia" ItemStyle-CssClass="field" />
                            <asp:BoundField DataField="GodzinaUruchomienia" HeaderText="Godzina Uruchomienia" ItemStyle-CssClass="field" />
                            <asp:BoundField DataField="IleNastapiloZmianRozmiaruZdjecia" HeaderText="Ile Zmian Rozmiaru" ItemStyle-CssClass="field" />
                            <asp:BoundField DataField="IleNastapiloZmianPolozeniaZdjecia" HeaderText="Ile Zmian Położenia" ItemStyle-CssClass="field" />
                            <asp:BoundField DataField="MinimalnyCzasReakcji" HeaderText="Czas Minimalny" ItemStyle-CssClass="field" />
                            <asp:BoundField DataField="MaksymalnyCzasReakcji" HeaderText="Czas Maksymalny" ItemStyle-CssClass="field" />

                        </Fields>
                    </asp:DetailsView>
                </asp:Panel>                
            </div>
            <div>
                <asp:Panel ID="MainPanel" runat="server" Visible="false">
                    <div class="container">
                        <div id="grid-container">
                            <div class="item"></div>
                            <div class="item border">
                                <asp:ImageButton ID="ImageButton1TK" runat="server" Width="100" Height="100" />
                            </div>
                            <div class="item"></div>
                            <div class="item border">
                                <asp:ImageButton ID="ImageButton4TK" runat="server" Width="100" Height="100" />
                            </div>
                            <div class="item menu">
                                <div class="item">
                                    <h3 class="text">Minimalny czas reakcji:</h3>
                                    <asp:Label ID="MinReactionTimeLabel" runat="server" Text="00:00,000" CssClass="text" ForeColor="red" />
                                    <h3 class="text">Maksymalny czas reakcji:</h3>
                                    <asp:Label ID="MaxReactionTimeLabel" runat="server" Text="00:00,000" CssClass="text" ForeColor="red" />
                                </div>
                                <div class="item">
                                    <asp:Button ID="ButtonSwitchTK" runat="server" Text="Przełącz rysunek" OnClick="ButtonSwitchTK_Click" />
                                    <h3 class="text" style="color: green !important">Zmiana rozmiaru rysunku</h3>
                                    <asp:RadioButtonList ID="ResolutionRadioTK" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ResolutionRadioTK_SelectedIndexChanged">
                                        <asp:ListItem Text="Mały rozmiar" />
                                        <asp:ListItem Text="Średni rozmiar" />
                                        <asp:ListItem Text="Duży rozmiar" Selected="True" />
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                            <div class="item border">
                                <asp:ImageButton ID="ImageButton2TK" runat="server" Width="100" Height="100" />
                            </div>
                            <div class="item"></div>
                            <div class="item border">
                                <asp:ImageButton ID="ImageButton3TK" runat="server" Width="100" Height="100" />
                            </div>
                            <div class="item"></div>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
        <asp:HiddenField ID="SessionEnded" runat="server" />
    </form>
</body>
</html>

