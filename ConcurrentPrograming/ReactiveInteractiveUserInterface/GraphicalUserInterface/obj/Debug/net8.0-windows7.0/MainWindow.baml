   M S B A M L   `   `   `  ����i  eTP.ConcurrentProgramming.PresentationViewModel, Version=4.2.0.0, Culture=neutral, PublicKeyToken=null�uclr-namespace:TP.ConcurrentProgramming.Presentation.ViewModel;assembly=TP.ConcurrentProgramming.PresentationViewModel/TP.ConcurrentProgramming.Presentation.ViewModel  5   	   - )TP.ConcurrentProgramming.PresentationView:   4TP.ConcurrentProgramming.PresentationView.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   �vmuclr-namespace:TP.ConcurrentProgramming.Presentation.ViewModel;assembly=TP.ConcurrentProgramming.PresentationViewModel     5   	     � Title$5  /Presentation layer targeting reactive behaviour��5   	   $	��550��6A   $	��450��6N    � DataContext 5	      I   CTP.ConcurrentProgramming.Presentation.ViewModel.MainWindowViewModel  5
   
   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   $��10q�6;   $��Center=�6G   .+�5      �� $��Number of balls:��6   $��Center=�60   $��0,0,5,0q�6K   �� 5       �� 
IsReadOnly$
 True��61   $��50��6C   ��6   �� *	BallCount+6C   �� 5      $��Start��6   $��80��5      $��0,0,5,0q�6#   $
��10,3q�64   ��5   %   �� *StartCommand+5   4   �� 5      $
��Stop��6   $��80��5      $
��10,3q�6#   ��5   $   �� *StopCommand+5   #   5      �� 5   
   $ 
Horizontal=�6   $��10q�6.   .+�5      �� $	��➖��6   $��20��6J   ��6!   �� *DecreaseBallsCommand+6J   �� 5      $	��➕��6   $��20��6J   ��6!   �� *IncreaseBallsCommand+6J   5       �� 5%   
   $��1��6   $��4q�6   
��� � �$	��410��5&      $	��410��6   $��10q�5'       �� CornerRadius$ 5��6   ���	#FFE8F5E9._�5(      �� $��0,0,0,0q�69   ��6   �� *Balls+��5)      �� 5*      .R�5+      �� 5,      ��5/      �� 50      .R�51      A� 
����  �
����  �$	��1.5��55   !   ��51   "   �� *
Diameter+��52   !   �� *
Diameter+ J� Effect 56      3  -System.Windows.Media.Effects.DropShadowEffect  57   "      ShadowDepth$ 2Q�63      Color$ DarkBlue��58   1   	   Opacity$		 0.6Q�59   1   
   
BlurRadius$
 5Q�5:   1   5<      5=      ��5@      �� 5A       �� 
TargetType" ��6   .�5B      ��  �� Property �@��Top �� Value 67   �� *Top+�� 5C       �@��Left 68   �� *Left+5D      5F      5G      5H      5I      