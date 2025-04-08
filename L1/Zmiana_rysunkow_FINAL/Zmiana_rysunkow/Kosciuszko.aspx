<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Kosciuszko.aspx.cs" Inherits="Zmiana_rysunkow.Kosciuszko" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="style.css" rel="stylesheet" />
    <title>Galeria zdjęć</title>
</head>
<body>
    <form id="formTK" runat="server">
        <asp:ScriptManager ID="scriptManagerTK" runat="server" />
        <div id="container">
            <div class="blue-bar"></div>
            <div id="grid-container">
                <div class="item"><asp:Label ID="labelIndexTK" Text="1" runat="server" CssClass="counter" /></div>
                <div class="item"><asp:ImageButton ID="imageButtonTK" runat="server" OnClick="ImageButtonTK_Click" ToolTip="Kliknij lewym przyciskiem myszy, aby wybrać zdjęcie." Width="400px" Height="300px" /></div>
                <div class="item"></div>
            </div>
            <asp:Button ID="buttonLeftTK" runat="server" Text="W lewo" Enabled="false" />
            <asp:Button ID="buttonRightTK" runat="server" Text="W prawo" Enabled="true" />
            <div class="blue-bar"></div>
        </div>
    </form>

</body>
</html>