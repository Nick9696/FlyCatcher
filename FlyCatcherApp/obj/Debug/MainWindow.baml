   M S B A M L   `   `   `  ����  DepthBasics-WPF5    /Microsoft.Samples.Kinect.DepthBasics.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Fly Catcher��5   	   $	��735��6   $��851.6��6*    � 	Resources 5         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� 
TargetType �� Property �� Value  SnapshotButton �� 
TargetType �� Orientation ��Name	 �� TextAlignment
 ��Name �� Triggers P� Property P� Value �� 
TargetName  CalibrationButton  SquareCheckBox �Name��%�  &	        &	 #     &	 F     '�� i     &	 �     7  7  &	 �    7  7  &	 A    7  7  �� 5   
   $ 	#ff6e6e6e��62   �� 5   
   $ 	#ff52318f��64   �� 5	   
   $ 	#ff00BCF2��62   �� 5
   
   " ��6   .�5      ��  �@J�SnapsToDevicePixels . 64   5      �� 5   
   " ��6   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      � .+�5      �� $ 
Horizontal=�6)   
������ .+�5   "   �� $ SnapText��6,   #��z��6>   $
	 Left=�6_   $��Center=�6u   8��  $��15(�6�   � 5   "   $��9,0,0,0q�6'   .+�5   &   �� $
 
SnapNormal��6,   $��Images\SnapNormal.png��6@   $
��None=�6_   $��Center=�6n   �� 5   &   $
 	SnapHover��6,   $��Images\SnapHover.png��6?   $
��None=�6]   $��Center=�6l   $y�	Collapsed=�6�   5   #   5      5       5      P� 5       �@J�IsMouseOver . . �5   "   �� $ 
SnapNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5   "   $ 	SnapHover��6O    � y�$ Visible=�6?   6O   �� 5   "   8  $ SnapText��6h    � ��5      5!      5#      5$      �� 5%   
   " ��6   .�5&      ��  � �� 5'      �� 5(      " ��6&   .R�5)      � .+�5*      �� $ 
Horizontal=�6)   
������ .+�5+   "   �� $ CalText��6,   #��z��6=   $
	 Left=�6^   $��Center=�6t   8��  $��15(�6�   � 5,   "   $��9,0,0,0q�6'   .+�5-   &   �� $
 UncheckedNormal��6,   $ ��Images\UncheckedNormal.png��6E   $
��None=�6i   $��Center=�6x   �� 5.   &   $
 CheckedHover��6,   $��Images\CheckedHover.png��6B   $
��None=�6c   $��Center=�6r   $y�	Collapsed=�6�   5/   #   50      51       52      P� 53       �@J�IsMouseOver . . �54   "   �� $ UncheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 55   "   $ CheckedHover��6O    � y�$ Visible=�6?   6O   �� 56   "   8  $ CalText��6h    � ��57      59      5;      5<      �� 5=   
   " ��6   .�5>      ��  � �� 5?      �� 5@      " ��6&   .R�5A      � .+�5B      �� $ 
Horizontal=�6)   
������ .+�5C   "   � $ SquareCheckBoxChecked��6'   .+�5D   &   �� $
 CheckedNormal��6,   $��Images\CheckedNormal.png��6C   $
��None=�6e   $��Center=�6t   �� 5E   &   $
 CheckedHover��6,   $��Images\CheckedHover.png��6B   $
��None=�6c   $��Center=�6r   $y�	Collapsed=�6�   5F   #   � 5G   "   $ SquareCheckBoxUnchecked��6'   $y�	Collapsed=�6H   .+�5H   &   �� $
 UncheckedNormal��6,   $ ��Images\UncheckedNormal.png��6E   $
��None=�6i   $��Center=�6x   �� 5I   &   $
 UncheckedHover��6,   $��Images\UncheckedHover.png��6D   $
��None=�6g   $��Center=�6v   $y�	Collapsed=�6�   5J   #   �� 5K   "   $ SquareCheckBoxText��6,   #��z��6H   $
	 Left=�6i   $��Center=�6   8��  $��15(�6�   $��9,0,0,0q�6�   5L      5M       5N      P� 5O       �@d�	IsChecked$ falseY�6;   . �5P   "   �� $ SquareCheckBoxChecked��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5Q   "   $ SquareCheckBoxUnchecked��6O    � y�$ Visible=�6?   6O   5R      P� 5S       �@J�IsMouseOver . . �5T   "   �� $ CheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5U   "   $ UncheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5V   "   $ CheckedHover��6O    � y�$ Visible=�6?   6O   �� 5W   "   $ UncheckedHover��6O    � y�$ Visible=�6?   6O   �� 5X   "   8  $ SquareCheckBoxText��6h    � ��5Y      5[      5]      5^      5_      .��5a      � -   5        $��
layoutGrid��5a      $��0 0 0 0q�6   .+�5b   
   ]� $��0��6   $��0,0,-0.2,636q�6!   $��2��67   $��2��6K   (� 5c   
   $��1��6    (� Stretch$ Uniform=�6   $
��Left=�61   $��2��6L   $��
0,0,-0.2,0q�6`   .��5d      �� -   5        $
��Game��5d      ��6!   ��  �� ElementName$ 
layoutGrid�� �� Path$ ActualWidth���6\   �� $ 
layoutGrid��$ ActualHeight���5e      �� 5f      -   5        	brush�� �� ImageSource$ Images/Background.jpg��5f   0    j� Stretch$
 Fill=�6T   5h      5i      (� 5j   
   $ Uniform=�6   $��Bottom=�6$   $
��Left=�6?   .��5k      �� -   5        $��Image��5k      $	��320��6!   $	��480��6-   $y�Visible=�6:   5l      5m      5n      