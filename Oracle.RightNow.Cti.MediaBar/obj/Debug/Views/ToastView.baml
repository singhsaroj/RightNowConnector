   M S B A M L   `   `   `  ����W  SOracle.RightNow.Cti.MediaBar, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Oracle.RightNow.Cti.MediaBar.Converters'Oracle.RightNow.Cti.MediaBar.Converters  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	   2    ,Oracle.RightNow.Cti.MediaBar.Views.ToastView   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   L
converters5clr-namespace:Oracle.RightNow.Cti.MediaBar.Converters     5   	   9System&clr-namespace:System;assembly=mscorlib     5   	   $	��100��5   #   $	��380��60     � 
ResizeMode$  NoResize=�5   	    � ShowInTaskbar .   � WindowStyle$
 None=�5	   	    � Topmost .  � WindowStartupLocation$ Manual=�5   	    � Title 5   	   �� *	Caption+ � 	Resources 5       �� MergedDictionaries �� Source   ApplicationSemiSubtleBrush	 �� Color
 �� Opacity  ApplicationFontSizeSmall " ApplicationFontSizeMediumLargeG   AOracle.RightNow.Cti.MediaBar.Converters.MediaTypeToImageConverter  MediaTypeToImageConverterH   BOracle.RightNow.Cti.MediaBar.Converters.MediaTypeToStringConverter  MediaTypeToStringConverter�� 5   
    5      �� 5      $9 3/Oracle.RightNow.Cti.Mediabar;component/Styles.xaml?�6%   %�   &	        &	 2     &	 L     &	 j     &	 }     �� 5      $	 	#FFBDBDBD��6A   $	
 0.8Q�6S   X�5      12Q�6A   X�5      
25.333Q�6K     5      63     5      64   5      .��5      �  � RowDefinitions 5   
   �� 5      $��60 �6   �� 5      $��40 �6    � ColumnDefinitions 5   
   �� 5      $��60 �6   �� 5      .+�5!   
   �� 
�������$��1q�5"      $��2��5#      $��2��5$      $��1q�5%      �� 5&   
   $��48��6   $��48��6   ��6'   ��  �� Path$ Interaction� �� 	Converter# [ �� 5'   
   $��1��5)      $��30��5*      $	��Top=�5+      $��Segoe UI)�5,      $��22(�5-      $��10,0,0,0q�5.      ��5'      �� $
 ress�5.      �� 5/   
   $��1��50      $��12(�51      ���	#FF007ACC$
��Wrap=�53      $��	10,5,10,0q�54      $��15��55      ��5/      �� $ Interaction� �� StringFormat$ New {0} interaction��# [ 55      �� 56   
   $��1��57      $��12(�58      $
��Wrap=�59      $��
10,35,10,0q�5:      $��15��5;      
���������56      �� $ Interaction.Queue�5<      �� 5>   
   $��1��6   $��1q�5?      $��40��5@      
�������$	��Top=�5B      $��2��5C      $	��1,2q�5D      �� 5E   
    �� Orientation$ 
Horizontal=�6   $��1��5F      $��Right=�5G      $��1��5H      .+�5I      ��   MediaBarButtonStyle#��[ 6   $��5q�6B   $��3��6N   $��26��5J      $��70��5K      
�������  ButtonNormalBorder#��[ 5N      $��1q�6G   ��5M      �� *AcceptCommand+.��5N   [   	_Accept5P      5Y      5Z      5[      