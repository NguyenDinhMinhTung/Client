   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  5   	       Client.ChatWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Client     5   	   -   5          � Title$  
ChatWindow��5   	   $	��450��6   $��368.639��6)    � WindowStyle$ 
ToolWindow=�6k    � 
ResizeMode$ NoResize=�6�    � ShowInTaskbar .  .��5	      �  � RowDefinitions 5
   
   �� 5      �� 5      $
��auto �6   �� 5      $
��auto �6   .+�5   
   �� -   5        $��ScrollViewer��5   $   $��0��6   $��	0 10 0 10q�68   .��5      �� -   5        $��
pnlChatBox��5      5      5      �� 5   
   $��1��6   $��	10 0 10 0q�6!   ]� 5   
   $��2��6   $��10 10 10 10q�6!   .+�5      �� -   5        $��btnSend��5      $��Right=�6$   $	��100��6;   $
��Send��6G   $��Bottom=�6V   6q   �� 5      -   5         ��Name$ 
txtMessage��5      $��0 0 10 0q�6*   $
��Left=�6<   5      5      5      