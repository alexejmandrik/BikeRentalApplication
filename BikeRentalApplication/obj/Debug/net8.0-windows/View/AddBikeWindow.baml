   M S B A M L   `   `   `  ����P  LBikeRentalApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:BikeRentalApplication.ViewBikeRentalApplication.View  5   	   .    (BikeRentalApplication.View.AddBikeWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   :local(clr-namespace:BikeRentalApplication.View     5   	   -   5        $��AddBikeWind��5
   	     � Title$/  )Добавление велосипеда��5   	   $	��450��6'   $	��550��64    � WindowStartupLocation$ CenterScreen=�5	       � 
ResizeMode$ CanMinimize=�5   	   .��5      � $��15q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� -   5         ��Name$ TitleTextBlock��5      $��0��6,   $/��)Добавление велосипеда��69   $��20(�6V   $
��Bold"�6d   $��Center=�6v   $��0,0,0,20q�6�   � 5   
   $��1��6   .+�5      �� -   5         ��Name$ AddBikePanel��5      .+�5      �� $��Название:��6   �� 5      -   5        $��	NameBlock��5      $��0,0,0,10q�6E   ��6+   �� *
BikeName+6E   �� 5      $��Описание:��6   �� 5      -   5        $��DescriptionBlock��5      $��0,0,0,10q�6S   ��62   �� *BikeDescription+6S   �� 5      $$��Полное описание:��6   �� 5      -   5        $��FullDescriptionBlock��5      $��0,0,0,10q�6[   ��66   �� *BikeFullDescription+6[   �� 5!      $��Путь к фото:��6   �� 5"      -   5        $��	PathBlock��5"      $��0,0,0,10q�6J   ��6+   �� *BikeImagePath+6J   �� 5$      $��	Цена:��6   �� 5%      -   5        $��
PriceBlock��5%      $��0,0,0,10q�6k   ��6P   �� *	BikePrice+6k   �� 5'      $��Добавить��6   $
��10,5q�6�   $��Stretch=�6�   ��6,   �� *
AddNewBike+��6K   ��  �� ElementName$ AddBikeWind��6�   5(      5)      �� 5+   
   $��2��6    �� Orientation$ Vertical=�6"   $��Center=�69   $��0,15,0,0q�6V   .+�5,      �� -	   5        $ StatusTextBlock��5,      
���  ��$��Center=�6B   $��0,5,0,0q�6_   $
��Wrap=�6p   5-      5.      5/      