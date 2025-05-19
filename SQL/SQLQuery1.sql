DELETE FROM Bikes;

INSERT INTO Bikes (name, description, ImagePath, Price, FullDescription)
VALUES 
('Cannondale Scalpel', N'Карбоносовый кросс-кантрийный байк с 100мм ходом', '/Resources/CannondaleScalpel.png', 1,
N'Год выпуска: 2023 Рама: BallisTec Carbon, FlexPivot, 100мм хода Вилка: Lefty Ocho, 100мм, воздушная Амортизатор: RockShox SIDLuxe Select+ Трансмиссия: Shimano XT/SLX, 12 скоростей Тормоза: Shimano Deore M6100, гидравлические дисковые Колёса: NoTubes Crest S1, 29" Покрышки: Schwalbe Racing Ray/Ralph, 2.25" Вес: 11.8 кг'),

('Cube Aim 29 Pro', N'Надёжный хардтейл для начинающих и тренировок', '/Resources/Cube.jpg', 2,
N'Год выпуска: 2023 Рама: Aluminium Lite, 29", внутренняя прокладка тросов Вилка: SR Suntour XCE28, 100мм Трансмиссия: Shimano Tourney/Altus, 2x8 скоростей Тормоза: Clarks M2, гидравлические дисковые Колёса: CUBE ZX20, 29" Покрышки: Schwalbe Smart Sam 2.25" Вес: ~14.5 кг'),

('Cube Stereo One 2', N'Двухподвес для трейлов и лёгкого эндуро', '/Resources/CubeStereoOne22Pro.jpg', 5,
N'Год выпуска: 2023 Рама: Aluminium Superlite, 130мм хода Вилка: RockShox Recon Silver RL Air, 130мм Амортизатор: RockShox Deluxe Select Трансмиссия: Shimano Deore, 1x12 Тормоза: Shimano BR-MT420/MT410 Колёса: CUBE EX30, 29" Покрышки: Maxxis Minion DHF/DHR II, 2.3" Вес: ~14.8 кг'),

('KTM Ultra Fun', N'Хардтейл для туризма и города', '/Resources/KTMUltraFun.jpg', 3,
N'Год выпуска: 2023 Рама: Aluminium Alloy 6061 Вилка: Suntour XCM HLO, 100мм Трансмиссия: Shimano Altus/Acera, 3x8 скоростей Тормоза: Shimano MT200, гидравлические Колёса: KTM Line CC 29" Покрышки: Schwalbe Rapid Rob 2.25" Вес: ~13.9 кг'),

('Orbea Avant H60', N'Шоссейник начального уровня на алюминиевой раме', '/Resources/OrbeaAvantH60.jpg', 1,
N'Год выпуска: 2023 Рама: Orbea Avant Hydro, алюминий Вилка: Carbon Трансмиссия: Shimano Claris, 2x8 Тормоза: Shimano Claris, ободные Колёса: Orbea Ready GR Покрышки: Vittoria Zaffiro V, 28mm Вес: ~10.8 кг'),

('Orbea OIZ H30', N'Лёгкий двухподвес для кросс-кантри', '/Resources/OrbeaOIZ_H30.jpg', 4,
N'Год выпуска: 2023 Рама: Hydroformed Alloy, 120мм хода Вилка: RockShox Recon RL 120mm Амортизатор: Fox iLine DPS Performance Трансмиссия: Shimano Deore/SLX, 12 скоростей Тормоза: Shimano MT201 Колёса: OC1 29c Покрышки: Maxxis Rekon Race 2.35" Вес: ~13.5 кг'),

('Orbea Orca M40', N'Кароновый шоссейник для продвинутых пользователей', '/Resources/OrbeaORCA_M40.jpg', 8,
N'Год выпуска: 2023 Рама: Carbon OMR Вилка: Carbon Трансмиссия: Shimano Tiagra 2x10 Тормоза: Shimano Tiagra, дисковые Колёса: Ready GR Покрышки: Vittoria Zaffiro V 28mm Вес: ~9.8 кг'),

('Trek Marlin 5', N'Начальный MTB с хорошей комплектацией', '/Resources/TrekMarlin5.jpg', 1,
N'Год выпуска: 2023 Рама: Alpha Silver Aluminum Вилка: RockShox Judy, 100мм Трансмиссия: Shimano Deore 10 скоростей Тормоза: Shimano MT200, гидравлические Колёса: Bontrager Connection 29" Покрышки: Bontrager XR2 Comp 2.2" Вес: ~14.3 кг'),

('Trek Slash 9.9', N'Топовый эндуро-байк для соревнований', '/Resources/TrekSlash9_9.jpg', 3,
N'Год выпуска: 2023 Рама: OCLV Mountain Carbon Вилка: RockShox ZEB Ultimate, 170мм Амортизатор: RockShox Super Deluxe Ultimate Трансмиссия: SRAM XX Eagle AXS, 12 скоростей Тормоза: SRAM Code RSC Колёса: Bontrager Line Pro 30 Покрышки: Bontrager SE5 Team Issue 2.6" Вес: ~13.9 кг'),

('Trek Stache 5', N'Фэтбайк-хардтейл с широкими колёсами', '/Resources/TrekStache2018.jpg', 10,
N'Год выпуска: 2017 Рама: Alpha Platinum Aluminum Вилка: Manitou Machete 32 Comp, 120мм Трансмиссия: SRAM NX 1x11 Тормоза: Shimano MT500, гидравлические Колёса: SUNringlé Duroc 40 SL Покрышки: Bontrager XR4 Team Issue 29x3.0" Вес: ~13.5 кг'),

('Cube Reaction C62', N'Лёгкий карбоновый хардтейл для гонок', '/Resources/CubeReaction_C62.jpg', 7,
N'Год выпуска: 2023 Рама: C:62 Carbon Вилка: RockShox Judy Silver TK, 100мм Трансмиссия: Shimano Deore 1x12 Тормоза: Shimano MT200 Колёса: Fulcrum Red77 Покрышки: Schwalbe Smart Sam 2.25" Вес: ~12.4 кг'),

('Trek Domane SL 5', N'Кароновый шоссейник с комфортной геометрией', '/Resources/Trek.jpg', 6,
N'Год выпуска: 2023 Рама: 500 Series OCLV Carbon Вилка: Domane SL carbon Трансмиссия: Shimano 105, 2x11 Тормоза: Shimano 105, гидравлические дисковые Колёса: Bontrager Affinity Disc Покрышки: Bontrager R1 Hard-Case Lite 32c Вес: ~9.9 кг');
