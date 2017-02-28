# ItsyBits
### DAT219 (Internet services on .net platform) project for [Ronny](https://github.com/Ronnrein), [Marielle](https://github.com/Nanilial) and [Nikolai](https://github.com/NikRob)
## Idea
The idea for our project is to have a web application/game that is about managing and developing a farm of tamagotchi-like animals.

For our project we have split the responsibilities as follows:

| Team member | Responsibility |
| --- | --- |
| Ronny | Backend development, server setup |
| Marielle | Project management, art, design |
| Nikolai | Frontend code and design |

## Getting started
Due to the relatively short time and scope of this project, as well as the fact that we have very defined roles and that the risk of merge conflicts are rather small, we will use a centralized workflow where we push directly to the main repository, using branches when it feels like it is needed.
### Setup
Go to the [repository](https://github.com/Ronnrein/ItsyBits) and press the "Clone or download" button, then copy the URl and clone the repository to your machine.
```shell
# If you use https
git clone https://github.com/Ronnrein/ItsyBits.git
# If you use ssh
git clone git@github.com:Ronnrein/ItsyBits.git
```
Now that you have a local copy of the project, you have to get the dependencies and migrate the database. Open up a command prompt, navigate to the project folder and type the following commands.
```shell
dotnet restore
dotnet ef database update
```
You are now set up to work on the project and push commits to the project.
### Workflow
The workflow will function very much as a private repository, except that you always need to make sure your clone is up to date with the repository on github. Therefore, it is wise to always do a pull before you start developing.
```shell
git pull origin master
```
Once this is done you can start working on the project. Once you are ready to commit your changes you could check the status of the git project.
```shell
git status
```
This might sometimes tell you that you have files that need to be added, if so add the files, either individually or all at once.
```shell
# Add individual file
git add Folder/File.html
# Add all unadded files
git add .
```
Sometimes it might also tell you that a file has been deleted. It will not let you add this deletion to a commit in a normal way, rather you must do one of the following.
```shell
# Remove specific file
git rm Folder/File.html
# Add all removed files (And all files in general)
git add -A
```
When this is done and the status command shows that all files are added, you can commit your changes
```shell
git commit -m "Explain your changes here"
```
Finally, you can now push your commit(s) to the repository
```shell
git push origin master
```
### Appendix
#### Merge conflicts
Sometimes you might run into something called a merge conflict. However, this is fairly unlikely to happen in this project, so i will not go into detail about it here, and instead ask you to let me know if that ever happens and i will help fix it.
#### Branches
Often when working on a project you may want to incorporate features without disturbing the main repository. This is where branches come in, and while they are an important part of git workflow in bigger projects, they are not as important here, so i will not go into detail about it.
