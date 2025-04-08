<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImageView.aspx.cs" Inherits="Zmiana_rysunkow.ImageView" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="style.css" rel="stylesheet" />
    <title>Widok obrazu</title>
</head>
<body>
    <form id="formImageTK" runat="server">
        <div id="container">
            <div class="gray-bar"></div>
            <div id="flex-container">
                <asp:Label ID="textTK" runat="server" />
                <asp:ImageButton ID="imageViewTK" OnClick="ImageViewTK_Click" runat="server" ToolTip="Kliknij lewym przyciskiem myszy, aby wrócić do wyboru zdjęć." Width="400px" Height="300px" />
            </div>
            <div class="gray-bar"></div>
        </div>
    </form>
</body>
</html>