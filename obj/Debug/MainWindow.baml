   M S B A M L   `   `   `  ����U  QLiveCharts.Wpf, Version=0.9.7.0, Culture=neutral, PublicKeyToken=0bc1f845d1ebb8dfG4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.WpfLiveCharts.Wpf  5   	    TelemetryViewer    TelemetryViewer.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Dlvc4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf     5   	     � Title$  TelemetryViewer��5   	   $	��700��6!   $
��1000��6.   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   � $��0��6   .+�5      ��  �� Orientation$ 
Horizontal=�6   $
��Left=�62   $
��0,10q�6M   .+�5      �� -   5         ��Name$ FilePathTextBlock��5      $��	10,10,0,0q�65    �� TextWrapping$
 Wrap=�6H   $	��Top=�6\   $	��699��6t   {�.  6�   �� 5      -   5        $��Открыть��5      $
��Left=�6+   $��	10,10,0,0q�6F   $	��Top=�6Y   $��22��6q   $��96��6}   6�   5      5      � 5   
   $��1��6   .+�5      �� $��1��6   $��10q�6&   .@�5      �� $��Кадр��6   .��5      �  5      �� 5      $
��Auto �6,   �� 5      $
��Auto �6,   �� 5      $
��Auto �6,   �� 5      $
��Auto �6,   .+�5      � $��0��6   .+�5       �� $.��(Служебная часть кадра��6$   $
��Left=�6D   $��0,0,0,0q�6_   $��Center=�6p   5!      � 5"      $��1��6   .+�5#      &   System.Windows.Controls.DataGrid  -   5          Name$ ServiceDataGrid��5#   '      AutoGenerateColumns .  $��1��6\      HeadersVisibility$
 None=�6i   $��10q�6�      
IsReadOnly . 	 J� IsHitTestVisible	 . |�.  
   GridLinesVisibility$

 None=�6�      SelectionUnit$
 Cell=�6�   5$      � 5%      $��2��6   .+�5&      �� $8��2Информационная часть кадра��6$   $
��Left=�6I   $��0,0,0,0q�6d   $��Center=�6u   5'      � 5(      $��3��6   .+�5)        -   5        $ FrameDataGrid��5)   '    .  $
 None=�6Z   $��10q�6s    . 	 . |�.  $

 None=�6�   $
 Cell=�6�    � 	Resources 5*   "   *  $System.Windows.Controls.DataGridCell �� 
TargetType �� Triggers P� Property P� Value �� Property �� Value��%�   '        �� 5+   &   "  6,    5-   *   P� 5.   .    �@ 
IsSelected . . �5/   2   ��  � ��
 �����6O   �� 50   2    � ��
 �   �6O   51   /   53   '   54   #   55      56      57      58      �� 59      $��График��6   .��5:      � .+�5;      #   LiveCharts.Wpf.CartesianChart  -   5        $��InfoPartChart��5;   -   &    LiveCharts.Wpf.Charts.Base.Chart   LegendLocation$ Right=�6d      Series 6B   �� *InfoPartSeries+   AxisX 5=   "      LiveCharts.Wpf.Axis     Title$ Позиция слова��6+      Labels 6A   �� *Labels+   AxisY 5@   "     $ Значение��6+   5B      5C      5D      �� 5E      $��Гистограмма��6   .��5F      � .+�5G        -   5          Name$ HistogramChart��5G   -   $ Right=�5I       5H      �� *HistogramSeries+ 5K   "     $ Значение��6+     5L   "      LabelFormatter 6+   �� *
XFormatter+ 5O   "     $ Частота��6+   5R      5S      5T      5U      5V      �� 5W   
   $ 
Horizontal=�6   $��2��6.   $
��Left=�6;   $��10q�6V   .+�5X      �� $ Vertical=�6   $��Center=�60   $
��0,10q�6M   .+�5Y      �� -   5         ��Name$ FrameSlider��5Y      $	��656��6.    �� TickPlacement$ BottomRight=�6b    �� IsSnapToTickEnabled .  �� TickFrequency$ 1Q�6�   �� 5Z      $ 
Horizontal=�6   $��Center=�66   $
��0,10q�6S   .+�5[      	� -   5         	�Name$ HexRadioButton��5[   "   $	��HEX��6:     d� 	IsChecked$
  TrueY�6H   6Y   	� 5\      -	   5        $ DecRadioButton��5\   "   $	��DEC��6:   6H   5]      5^      �� 5_      $ Vertical=�6   $
��Left=�60   $��10q�6K   .+�5`      �� -
   5        ! ��Name$! CurrentFrameTextBlock��5`      $��10,0,0,0q�6;   $��Center=�6M   �� 5a      -   5        $! CurrentPositionTextBlock��5a      $��10,0,0,0q�6>   $��Center=�6P   5b      5c      5d      5e      