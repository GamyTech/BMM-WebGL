auto tools:
	There is an automatic tool that always watch if for any reason the platform change and then remanage the appropriate folders in asset or outside.
	This tool also keep the main scene active permanently.
	If for any reason you think the automatic process wasn't successfully made you can update everything manually by clicking on GTTools/Update App Settings. 
	To Disable that tool, disable GTTools/Enable AutoTools.

Working with SVN:
	The position of some assets in the project might depend on using the AssetBundle or not, the platform and the GameId.	
	For that reason, it is very important to set a specific state of the unity project before commiting.
	That state was decided as Backgammon 4 money on Android with using AssetBundle.
	A quick way to set up your project at this state is clicking on GTTools/Go to SVN state.