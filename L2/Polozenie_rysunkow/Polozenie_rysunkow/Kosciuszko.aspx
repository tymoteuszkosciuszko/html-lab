<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Kosciuszko.aspx.cs" Inherits="Polozenie_rysunkow.Kosciuszko" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="style.css" rel="stylesheet" />
    <title>Położenie rysunków</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scriptManagerTK" runat="server" />
        <div id="container">
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
    </form>
</body>
</html>
