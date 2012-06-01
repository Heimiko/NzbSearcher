Latest Version: 2.0.5.6

NzbSearcher is a windows client for SABnzbd which integrates it with various UseNet (nzb) search sites. Currently supported NZB search sites: NZBindex.nl, NzbMatrix.com, YabSearch.com and NZB.su

Install Instructions:
 - Make sure you have .NET 3.5 installed
 - Extract NzbSearcher anywhere on your computer, simply run it, no install requiered
 - have fun!

Known Limitations/Bugs:
 - NzbIndex.nl is the only fully implemented Search Engine, the others still have some short comings

NzbSearcher Release History

Release v2.0.5.6
 - fixed deletion of movies from imdb lists
 - fixed issue with computers with a locale using comma as decimal separator.
 - fixed issue with read-only configuration file. 
 - added a "Folder Watcher":
   NzbSearcher will upload all nzb files from a defined folder to SABnzbd, and monitor for future addition of nzbs. 
   It's usefull for user with SABnzbd running on a different machine from the one running NzbSearcher.

Release v2.0.5.5
 - fixed issues due to changes from imdb.com
 - version checking on sourceforge.net instead of sabnzbd.org

Release v2.0.5.4
 - added deletion of movies from IMDB watchlist
 - added shortcut keys for context items (watchlist)
 - automatic crash report sending without local email client (direct sending)

Release v2.0.5.3
 - added "Search in WebLink" to NzbMatrix provider
 - IMDB watchlist now searches for IMDB ID's when on NzbMatrix provider tab instead of movie title
 - now capturing more exceptions on fatal crash
 - fixed hard to trace bug which has been pestering many users ("all" provider crash when searching)
 - fixed NzbMatrix size differece when native system notation is 1.234,56 instead of 1,234.56
 - increased default timeout on web actions to 30 seconds
 - other small fixes

Release v2.0.5.2
 - better exception handling and reporting

Release v2.0.5.1
 - added fetching of all IMDB's lists you may have created (no manual addign needed for those)
 - added double click on SAB tab button for opening website
 - fixed audio (ping) when pressing enter in the searchbox
 - NZB.su doesn't support min size, max days in its API, greyed them
 - fixed crash when HTTP page not found (404 error) in any scenario
 - other small fixes

Release v2.0.5.0
 - added IMDB Watchlist support (different than IMDB My Movies)
 - IMDB My Movies button made optional (default off)
 - now initial retrieved item count when searching is configurable
   more items mean longer loading time, but more items available right away.
 - fixed bug on IMDB My Movies menu rebuilding
 - the toolbar positions are now also saved within the same application config file,
   meaning that if you configure it to be a portable configuration, it truely is,
   nothing will be stored locally anymore (no registry, no other files)
 - other small fixes

Release v2.0.4.4
 - added portable config option
 - fixed YabSearch downloaded NZB files
 - other small fixes

Release v2.0.4.3
 - fixed YabSearch provider (got broken by site update)
 - YabSearch now supports proper sorting
 - fixed HTML parser bug
 - fixed IMDB crash when movie had unknown year
 - UseNetServer now has propper age values
 - possibility to use UseNetServer GlobalSearch with "All" provider (although still not recommended)
 - better way of sorting for collective (all) provider, sorting now actually retrieves new sorted items from its sub-providers

Release v2.0.4.1
 - added UseNetServer GlobalSearch provider  (should not be used with "All" search provider)
 - fixed several bugs in "All" search provider, causing issues with filtering items, displaying too few results,
   and resulting in auto downloader not always working properly
 - removed download category from all search providers, this can now only be specified in the download dialog
 - better Tab configuration page (listview)
 - allow '/' in friendly name (download dialog) for setting passwords
 - failed download check improved (auto downloader)

Release v2.0.3.4
 - Auto Downloader: now using the "All" provider implementation, as NzbIndex.nl started to miss a lot.
 - Auto Downloader: NzbMatrix.com not used when using non-alfa-numeric chars in search, because it'll return weird results
 - Auto Downloader: Configurable delayable auto-search for recent and old episodes, because some search engines have search limits (nzbmatrix, nzb.su)
 - better thread seperating gui from background work (Auto downloader) -> no more GUI lockups
 - able to configure "All" Provider independant of enabled tabs (see "tabs" config page)
 - fixed column sorting on "All" provider
 - reset IMDB filter on re-opening menu

Release v2.0.3.1
 - Fixed critical bug in IMDB configuration page.  No one (but me) was actually able to use it.  this is now fixed. (sorry folks)
 - Added "Search All Providers" searcher page. This page will search all enabled providers, returning only a limited amount of items.
 - other small bug fixes

Release v2.0.2.0
 - Added complete IMDB "My Movies" integration! (on search tabs)
   For this to work correctly, in the main page of your "My Movies" (on IMDB), go to "Change display options",
   then enable "Show -> All Titles", otherwise only the first X amount will be shown in NzbSearcher.
 - Minor bugs fixed

Release v2.0.1.5
 - Auto Downloader now has option to put in a regular expression result filter.
   This is useful when normal search would give too many (and unreliable) results.
   ie. for the series '24', you could add result filter: 24.S%SE%E
   in this example, the item must have '24' then any single char, then S01E01 (or whatever episode is looked up)
   Same goes for Fairy Tail, which doesn't has a season number, which could look like this: Fairy Tail[ -0]*%E
   remember, a space in the regular expression syntax really is a space.
 - NzbSearcher can now open NZB files directly (local) and pass them on to SAB. (so it's now a file opener too)
   this only works correctly at the moment when NzbSearcher is already running in the background.
 - several bugs and annoyances fixed
 - Search toolbar can now optionally be static on the dialog again (so not actually in a toolbar anymore)
   as it was previously before version 2.0.     this can be toggled in the config dialog.
   I've added this option, cause I've noticed that the new toolbar-mode gave focus issues.

Release v2.0.1.3
 - Fixed bug which prevented NzbSearcher to startup in specific cases

Release v2.0.1.2
 - Toolbar locations restored on program restart (only on top docking positions though)
 - Multi selection of NZB's now downloading as a single item (works on all search providers)
 - SABnzbd queueu control now has "Move to Top" and  "Move to Bottom" buttons (also works with multi-select, just like normal move)
 - Speed limiter of SABnzbd now visible and directly controllable from NzbSearcher

Release v2.0.0.0:
 - Better GUI layout, draggable toolbars
 - Better organized configuration storage (WARNING: makes V2 incompatible with V1, make sure you remember your settings)
 - Automatic downloaded articles now includes titles with friendly name (%T now works)
 - NZB.su search support (not fully completed, but workable)
 - Enable/disable tabs, any search engine (or other tab) can be hidden.
 - Auto Downloader much better, will confirm with SABnzbd if article downloaded and extracted, before moving on to next episode.
 - Auto Downloader has now options to check for next episode and season when unable to find current one.
 - Optionally downloading NZB, then uploading it to SABnzbd.
   The "Auto Downloader" always uses this option, as it is better able to tell what's happening,
   except of course when configured to store to a directory or open with a program, in that case,
   as soon as the NZB is downloaded, it will mark the episode as "done" and moves on to the next.
 - Optionally save files to a location, instead of passing them to SABnzbd
 - Optionally open a program with the saved NZB file as parameter
 - Optionally disable SABnzbd integration entirely (when hiding SABnzbd's tab)
 - SABnzbd icon will blink when downloading. Yellow only is passive state.
 - Red SABnzbd icon state means there's a failure with downloading, so you can check out the message on the SABnzbd screen.
 - Icons made higher resolution
 - added crash report system, whenever NzbSearcher crashes, it can now make a report, so that I can help you fix your problem.
 - added config incompatibility warning
 - lots and lots of small improvements and fixes

Release v1.2.2.0
 - Added YabSearch.com search support
   No proper NFO support yet, but that'll be implemented soon-ish.

Release v1.2.0.0
 - Added NzbMatrix search support.
   Be sure to fill in NzbMatrix details in config dialog, and also in SABnzbd's config!
   Need to have VIP access for this, is a one time fee of $10 i believe.
   Due to incomplete API, NFO files can't be viewed. Working on solution.
   Due to incomplete API, results can't be sorted. can't be helped. you can select a sorting method in your NzbMatrix profile.
   NzbMatrix is using HTTPS for a secure connection
 - Added colors to Auto Downloader (same as favorites, green en red colored for 6-8 and 8-15 days)
 - Added availability column (mainly useful with NzbIndex.nl for now)
 - Fixed flickering when minized into tray (when SABnzbd was downloading)
 - Fixed add to queue SABcat change, when going back from add to queue dialog, category is passed along back to search dialog
 - Fixed selection issue when opening NFO screen with focus

Release v1.1.3.2
 - Added toggle button to show/hide NFO file contents from NZB files (shows whenever info is available)
 - Added NFO link in context menu, opens in notepad (also with Ctrl+F11)
 - Added IMDB access in context menu, opens in external browser (also with Ctrl+F10)
   when NFO is available for current selected, it'll download that first, check if there's a IMDB link in it,
   if so, it'll use that to directly jump to the correct page. Otherwise, It'll ask you to alter the title to search for.
 - Downloaded history (SABnzbd tab) can now be double clicked, it opens the storage location
 - Question if you really want to quit when you have configured any automatic downloads,
   now only pops up when you have enabled NzbSearcher to sit in system tray.
 - some other small stuff I can't think of right now.

Release v1.1.1.1
 - Added speed indication
 - Added space indication
 - added about-box with link to website
 - result filter fixed to work with minus (ie. -german will only show nzb's without german in it)
 - after search, focus is now set on result list
 - bugs fixed
 - other stuff I can't remember

Release v1.1.0.1
 - Fixed bug that many of you reported. Thanks for everyone who sent this in.

Release v1.1.0.0
 - BIG addition: automatic episode downloader
 - App can now sit in the system tray (enable this in the config dialog)
 - number of small additions and bug fixes

Release v1.0.6.6
 - Bug fix release

Release v1.0.6.4
 - Added version checking, user will be notified if new version is released
 - Download size can now be displayed in KB. this is optional (turn on in config dialog)
 - Added favorite colored star. This star is an indicator of when last download took place.
   When green, the favorite hasn't downloaded an article for 6 to 8 days
   When red, nothing has been downloaded for 8 to 15 days
   No star means either less then 6 days or more then 15 days,
   in which case user can pay less attention to it.
   Ofcorse the timestamp of the article is used (not the actual time of the download)
   This addition is most useful for weekly recurring downloads (series for example)

Release v1.0.5.4
 - Added SABnzbd download history.
   This is done intelligently, so that the history isn't refreshed every single second.
   So when you also use other programs with SABnzbd, the history might get out of date.

Release v1.0.4.4
 - Added the most important SABnzbd controls: delete, move up, move down. (more later)
 - Added IMDB button in "Add to Queue" dialog. (will be moved somewhere else in future)
 - bug fixes

Release v1.0.3.4
 - Fixed sorting issue when not filtering a list

Release v1.0.3.3
 - Added splash screen
 - Added result filter
 - Added on the fly loading of more articles.
   When you scroll down, more articles are fetched on the fly, when more is available.
   this only works when list is not filtered by the "result filter".
   This will be done with 25 items at a time.
   Most usually, when you search, 50 or 75 items are loaded, because more then 25 items fit on the screen.
   With this addition, true browsing of specific groups is possible!
   (to do this, sort by age first, then specify the groups you want to browse, then do an empty search)

Release v1.0.2.3
 - Added possibility to specify which groups to search in (NzbIndex.nl) ofcorse these will be saved within favorites as well.
 - Fixed a minor bug in SABnzbd group pickup from settings when prepairing download
 - Added "New Search" button, to easily clear all search data
 - minor bug fixing

Release v1.0.1.1
 - Improved favorites handling
 - Added SABnzbd categories to search dialog, will be saved within favorites

Release v1.0.0.1
 - Fixed issue when SABnzbd webserver returned an error

Release v1.0.0.0
 - Initial release
 - Adds NZBindex.nl search
 - Favorites storage
 - Able to select category before NZB download / import
 - SABnzb can only be viewed, not really controlled yet