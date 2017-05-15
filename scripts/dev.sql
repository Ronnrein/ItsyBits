/*
    Now run 'dotnet ef database update' to migrate the tables
    Afterwards, run this as root on your dev server
    After this, you can start the server and login using username and password 'admin'
*/
USE itsybits;
INSERT INTO `aspnetroles` (`Id`,`ConcurrencyStamp`,`Name`,`NormalizedName`) VALUES (
    'be9e1ee4-f6ab-435d-afd9-a514d3ba6759','914abd25-a11b-4fb7-80cb-c4c58650484d',
    'Administrator','ADMINISTRATOR'
);
INSERT INTO `aspnetusers` (`Id`,`ConcurrencyStamp`,`Currency`,`Email`,`EmailConfirmed`,`NormalizedEmail`,`NormalizedUserName`,`PasswordHash`,`SecurityStamp`,`UserName`,`AccessFailedCount`,`LockoutEnabled`,`PhoneNumberConfirmed`,`TwoFactorEnabled`) VALUES (
    '76877613-be5e-4d36-b3a3-15dd93643760',
    'fcaee553-12d0-4f83-aee1-c75001c5fec7',
    1000,
    'admin@admin.admin',
    1,
    'ADMIN@ADMIN.ADMIN',
    'ADMIN',
    'AQAAAAEAACcQAAAAENEEY8WOYGzYFoXB1B6TYDccHon5Y4THsAlzCrdCbJPkTRhsxvOOYF09Z2dH+Faq9Q==',
    '39209308-cb37-431e-a690-909bf14ccba9',
    'admin',
    0,
    0,
    0,
    0
);
INSERT INTO `aspnetuserroles` (`UserId`,`RoleId`) VALUES (
    '76877613-be5e-4d36-b3a3-15dd93643760',
    'be9e1ee4-f6ab-435d-afd9-a514d3ba6759'
);
INSERT INTO `plots` (`Id`,`Description`,`PositionX`,`PositionY`) VALUES (
    1,
    'Leftmost',
    300,
    200
);
INSERT INTO `buildingtypes` (`Id`,`Capacity`,`MaxCapacity`,`Name`,`Price`,`SpritePath`) VALUES (
    1,
    3,
    5,
    'Shack',
    50,
    'shack'
);
INSERT INTO `buildings` (`Id`,`Name`,`PlotId`,`TypeId`,`UserId`) VALUES (
    1,
    'The shack',
    1,
    1,
    '76877613-be5e-4d36-b3a3-15dd93643760'
);
INSERT INTO `animaltypes` (`Id`,`FeedTime`,`LevelMultiplier`,`Name`,`PetTime`,`Price`,`SleepTime`,`SpritePath`) VALUES (
    1,
    '22:00:00.000000',
    1,
    'Bear',
    '15:00:00.000000',
    30,
    '24:00:00.000000',
    'bear'
);
INSERT INTO `animals` (`Id`,`BuildingId`,`Created`,`LastFeed`,`LastPet`,`LastSleep`,`Level`,`Male`,`Name`,`TypeId`) VALUES (
    1,
    1,
    NOW(),
    NOW(),
    NOW(),
    NOW(),
    1,
    1,
    'Winnie',
    1
);
INSERT INTO `notifications` (`Id`,`Created`,`Image`,`IsRead`,`Link`,`Message`,`Title`,`UserId`) VALUES (
    1,
    NOW(),
    'misc/coins.jpg',
    0,
    '/store',
    'GJ, you imported the dev database!',
    'Caching!',
    '76877613-be5e-4d36-b3a3-15dd93643760'
);
INSERT INTO `upgrades` (`Id`,`CapacityModifier`,`Description`,`FeedModifier`,`ForAnimal`,`ForBuilding`,`Method`,`Name`,`PetModifier`,`Price`,`SleepModifier`,`SpritePath`) VALUES (
    1,
    0,
    'Comfy blanket that decreases the need to be pet',
    1,
    1,
    0,
    NULL,
    'Comfy blanket',
    1.5,
    10,
    1,
    'blanket.png'
),(
    2,
    1,
    'Adds 1 room to a building',
    1,
    0,
    1,
    NULL,
    'Building extension +1',
    1,
    20,
    1,
    'extension.png'
);
INSERT INTO `buildingupgrades` (`Id`,`BuildingId`,`UpgradeId`) VALUES (1,1,2);
INSERT INTO `animalupgrades` (`Id`,`AnimalId`,`UpgradeId`) VALUES (2,1,1);
