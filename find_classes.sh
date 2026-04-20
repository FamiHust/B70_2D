@echo off
cd /d c:\B70_2D
echo Searching for ItemData class definition...
findstr /R /S "class ItemData" *.cs Assets\*.cs Assets\_Project\*.cs Assets\_Project\Scripts\*.cs 2>nul
echo.
echo Searching for ItemsCollection class definition...
findstr /R /S "class ItemsCollection" *.cs Assets\*.cs Assets\_Project\*.cs Assets\_Project\Scripts\*.cs 2>nul
echo.
echo Searching for max level logic...
findstr /R /S /I "max.*level\|level.*limit\|MAX_LEVEL\|MaxLevel" *.cs Assets\*.cs Assets\_Project\*.cs Assets\_Project\Scripts\*.cs 2>nul
