   M S B A M L   `   `   `  ����P  LBikeRentalApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:BikeRentalApplication.HelpersBikeRentalApplication.Helpers  5   	   Q-clr-namespace:BikeRentalApplication.ViewModelBikeRentalApplication.ViewModel  5   	   +    %BikeRentalApplication.View.AuthWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   >helper+clr-namespace:BikeRentalApplication.Helpers     5   	   C	viewModel-clr-namespace:BikeRentalApplication.ViewModel     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -   5        $��AuthWind��5	   	     � Title$5  /Авторизация / Регистрация��5
   	   $	��580��6+   $	��450��68    � WindowStartupLocation$ CenterScreen=�5   	    � 
ResizeMode$ CanMinimize=�5   	    � DataContext 5      ,   &BikeRentalApplication.ViewModel.AuthVM  5   
    � 	Resources 5         BoolToVisibilityConverter��%   &	        �� 5   
   6'   5      .��5      � $��15q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��20(�6<   $
��Bold"�6J   $��Center=�6\   $��0,0,0,10q�6y   ��6!   �� *	TitleText+6y   � 5"      $��2��6   .+�5#      �� y�6   �� *IsLoginVisible+ �� 	Converter# [  .+�5$      �� $��
Логин��6   �� 5%      $��0,0,0,10q�6b   ��6   �� *LoginUsername+ �� UpdateSourceTrigger$ PropertyChanged=�6b   �� 5'      $��Пароль��6   G� 5(      5   /BikeRentalApplication.Helpers.PasswordBoxHelper   BindPassword$
 True��6"   $��0,0,0,10q�5*      	   BoundPassword	 5)      �� *
Password+
 �� Mode$
 TwoWay=�$ PropertyChanged=�5*      �� 5,      $��
Войти��6   $
��10,5q�6J   $��Stretch=�6Y   ��6)   �� *LoginCommand+6Y   �� 5-      $��1��6   $��/Resources/MainBike.jpg��5.      $	��170��5/      $��Uniform=�50      $��	0,75,0,15q�51      $��Center=�52      53      �� 55      y�6   �� *IsRegisterVisible+# [  .+�56      �� $��Логин *��6   �� 57      $��0,0,0,10q�6e   ��6   �� *RegisterUsername+$ PropertyChanged=�6e   �� 59      $��Фамилия *��6   �� 5:      $��0,0,0,10q�6d   ��6   �� *RegisterSurname+$ PropertyChanged=�6d   �� 5<      $��Имя *��6   �� 5=      $��0,0,0,10q�6a   ��6   �� *RegisterName+$ PropertyChanged=�6a   �� 5?      $��Отчество��6   �� 5@      $��0,0,0,10q�6g   ��6   �� *RegisterPatronymic+$ PropertyChanged=�6g   �� 5B      $#��Номер телефона *��6   �� 5C      $��0,0,0,10q�6b   ��6   �� *RegisterPhone+$ PropertyChanged=�6b   �� 5E      $��Пароль *��6   G� 5F      -   5         G�Name$ RegisterPasswordBox��5F   "   $
 True��5G      $��0,0,0,10q�5I      	 5H      �� *RegisterPassword+$
 TwoWay=�$ PropertyChanged=�5I      �� 5K      $/��)Подтверждение пароля *��6   G� 5L      -   5        $  RegisterConfirmPasswordBox��5L   "   $
 True��5M      $��0,0,0,15q�5O      	 5N      �� *RegisterConfirmPassword+$
 TwoWay=�$ PropertyChanged=�5O      �� 5Q      $*��$Зарегистрироваться��6   $
��10,5q�5R      $��Stretch=�6(   ��5Q   :   �� *RegisterCommand+5R   (   5S      5T      �� 5V      $��3��6    �� Orientation$ Vertical=�6&   $��Center=�6=   $��0,15,0,0q�6Z   .+�5W      �� $��Center=�6   �� 5X       �� Command 6   �� *SwitchCommand+.�6>   .�5Y      �� �6   �� *SwitchLinkText+5Z      5[      �� 5\      
���  ��$��Center=�6-   $��0,5,0,0q�6J   $
��Wrap=�6[   $��20��6o   ��5]      �� *StatusMessage+5^      5_      5`      