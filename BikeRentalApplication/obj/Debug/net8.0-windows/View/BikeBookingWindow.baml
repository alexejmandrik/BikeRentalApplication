   M S B A M L   `   `   `  ����P  LBikeRentalApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:BikeRentalApplication.ViewBikeRentalApplication.View  5   	   Q-clr-namespace:BikeRentalApplication.ViewModelBikeRentalApplication.ViewModel  5   	   2    ,BikeRentalApplication.View.BikeBookingWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   :local(clr-namespace:BikeRentalApplication.View     5   	   <vm-clr-namespace:BikeRentalApplication.ViewModel     5   	     � Title$3  -Бронирование велосипеда��5	   	   $	��400��6)    � WindowStartupLocation$ CenterOwner=�5
   	    � SizeToContent$ Height=�5   	    � 
ResizeMode$ NoResize=�5   	   .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $3��-Бронирование велосипеда��6!   $��20(�5      $
��Bold"�6"   ���	#FF1F1F1F$��Center=�5      $��0,0,0,20q�61   �� 5   
   $��1��6   $	��Top=�6"   .+�5      �� $��Велосипед:��6   $��SemiBold"�6*   $��0,0,0,5q�6@   �� 5       �� 
IsReadOnly$
 True��67   ���	#FFF0F0F0$��0,0,0,10q�6`   ��6   �� *BikeToBook.Name+6`   � 5      $��0,10,0,0q�6    � ColumnDefinitions 5      �� 5      $��* �6'   �� 5      $
��Auto �6'   .+�5"      �� $��0��6   $��0,0,10,0q�6-   .+�5#      �� $��Дата начала:��6    $��SemiBold"�64   $��0,0,0,5q�6J   (  "System.Windows.Controls.DatePicker  5$         SelectedDate 6!   �� *	StartDate+ �� UpdateSourceTrigger$ PropertyChanged=�5%      �� 5'      $��1��6   .+�5(      �� $��Время начала:��6    $��SemiBold"�65   $��0,0,0,5q�6K   �� 5)      	 �� ItemStringFormat$	 	{0:00}:00��5+      $��80��5,      ��5)      �� *AvailableHours+
 �� SelectedItem
 5*      �� *	StartTime+$ PropertyChanged=�5,      5-      5.      � 50      $��0,15,0,0q�6    51      �� 52      $��* �6'   �� 53      $
��Auto �6'   .+�56      �� $��0��6   $��0,0,10,0q�6-   .+�57      �� $"��Дата окончания:��6    $��SemiBold"�67   $��0,0,0,5q�6M     58       6!   �� *	EndDate+$ PropertyChanged=�59      �� 5;      $��1��6   .+�5<      �� $$��Время окончания:��6    $��SemiBold"�68   $��0,0,0,5q�6N   �� 5=      $	 	{0:00}:00��5?      $��80��5@      ��5=      �� *AvailableHours+
 5>      �� *	EndTime+$ PropertyChanged=�5@      5A      5B      �� 5D       �� Orientation$ 
Horizontal=�6   $��0,15,0,0q�62   .+�5E      �� $��Стоимость:��6   $��SemiBold"�6.   �� 5F      �� 5G      �6   �� *	TotalCost+ �� Mode$ OneWay=� �� 5H      $	� Br��6   5I      5K      �� 5N      $��0,15,0,0q�6   .+�5O      �� $ 
Horizontal=�6   $��0,0,0,5q�66   .+�5P      �� $$��Доступные баллы:��6    $��SemiBold"�68   �� 5Q      ���#5C8F45$��14(�6V   $��5,0,0,0q�6d   ��6    �� *AvailablePoints+6d   5R      �� 5T      $ 
Horizontal=�6   $��Center=�66   $��0,10,0,0q�6Q   .+�5U      �� $%��Оплатить баллами��6    d� 	IsChecked 5V      �� *	UsePoints+$ PropertyChanged=��� 5W      $��1Br = 10 бонусов��6    
�������$��10,2,0,0q�6J   $��12(�6\   5X      5Y      5Z      �� 5\   
   $��2��6   $��0,10,0,0q�6"   .+�5]      �� 
���  ��$��12(�5_      $
��Wrap=�5`      $��0,0,0,10q�5a      ��5]      �� *ErrorMessage+5a      �� 5c      $ 
Horizontal=�6   $��Right=�62   .+�5d      �� $��Отмена��6   $	��100��6*   $��0,0,10,0q�66    �� IsCancel$
 True��5e   3   ��6   �� *CancelCommand+63   �� 5f      $ ��Забронировать��6   $	��120��61   ���#5C8F45
������� �� 	IsDefault$
 True��5g   ;   ��6   �� *ConfirmBookingCommand+6;   5h      5i      5k      5l      