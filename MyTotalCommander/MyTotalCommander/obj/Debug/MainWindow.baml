   M S B A M L   `   `   `  ����  MyTotalCommander    Total.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$  File Manager��5   	   $��702.266��6   $��1056.295��6/   .��5      � -   5         �Name$
 grid��5       � RowDefinitions 5	   
   �� 5
      $��40 �6   �� 5      $��5 �6   �� 5      $
��600* �6   �� 5      $��2 �6   .+�5   
   �� -   5         ��Name$ border��5      $��2��6!   $��0,50,0,0q�6.   �� 5   
   -   5         ��Name$ 
stackPanel��5      $��0��6)   $��5q�66   $��60��6A   .+�5      o� $
��Left=�6   $��Center=�6.   ��6I   �� *
Background+ �� ElementName$
 grid��.@�5      m� -   5        $��menuitmCopy��5      $��5q�6.   $��Копировать��69   $��16(�6M   ���	#FFDE67676u   5      5      �� 5   
   $��1��6   $��Stretch=�6!   $��Stretch=�6F   � 5   
   $��2��6   
������� � ColumnDefinitions 5      �� 5      $
��200* �6#   $	��250��60   �� 5      $��5 �6#   �� 5      $
��200* �6#   $	��250��60   .+�5      �� -   5        $ stackPanel1��5       �� Orientation$ Vertical=�6.   $��0��6E   $��2q�6U   ��6`   Z� .+�5      o� -   5        $��menuDiscBtn��5      $��Stretch=�6*   $��Stretch=�6O   $��2q�6r   ���	#FF7171E8���	#FFF225256�   �� 5      -   5        $��tblkWay��5      &   System.Windows.Controls.DataGrid  5      -   5        $��dataGrid��5         AutoGenerateColumns .  "	   VerticalScrollBarVisibility$	 Visible=�6�   
 � UseLayoutRounding
 .  � IsSharedSizeScope . $   HorizontalScrollBarVisibility$ Visible=�61     CanUserResizeRows .     MinRowHeight$ 10Q�6s     AreRowDetailsFrozen . 
������� � DataContext 6G   �� $ dataGrid�� �� Path$ Items���6�  �� *ActualHeight+$ border�� �� Mode$ OneWay=�   Columns 5       0  *System.Windows.Controls.DataGridTextColumn  5!      ,  &System.Windows.Controls.DataGridColumn   Header$ Имя��6K   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6-   �� $
 Name�6K     5"      $ Тип��6K    6-   �� $
 Type�6K     5#      $ Размер��6K    6-   �� $
 Size�6K   5%      5&      �� 5'      $��1��6   $��Stretch=�6+   $��2��6N   $��Stretch=�6X   $��Stretch=�6}   ���	#FF8B8282���	#FFCDBCBC6�   �� 5(      $ Vertical=�6   $��2��60   $��2q�6@   .+�5)      �� -	   5        $��ComboBox��5)      5*      R� 5+      -
   5        $��TreeVW��5+      ��. $��Visible=�6N   $��Visible=�6�   ���	#FFF8F8F8��6�   �� *ActualHeight+$ border��$ OneWay=�5,      5-      5.      5/      50      