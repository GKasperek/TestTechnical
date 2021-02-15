# Test de développement

Le but de ce test est de déterminer votre niveau de maitrise d'Unity, du C#, du GamePlay et de la gestion de projet mobile.

## Créer un jeu de taquin.

Vous devez créer un jeu de taquins dans une nouvelle scène Unity et la connecter à la "Home". Les règles du jeu sont les règles traditionnelles  (https://fr.wikipedia.org/wiki/Taquin ) adaptées pour 9 cases et utilisant une image en guise de cible finale. Comme pour les jeux de taquins pour enfants.

<img src="https://github.com/jfc-babaoo/Test/raw/release/README.Assets/taquin_kid.jpg" width="200">

⚠️ Attention : La case du centre sera la case vide à la résolution pour notre jeu.



## Les instructions sont les suivantes
- "checkout" du dépôt git https://github.com/jfc-babaoo/Test.git
- créer une nouvelle scène et la rentre accessible par le bouton sur la "home"
- le jeu doit commencer par un tirage aléatoire des positions de cases
- le tirage doit être soluble
- le joueur doit déplacer les cases par glisser-déposer tactile
- le joueur à 3 minutes pour réussir
- le meilleur score doit être affiché sur la home et enregistré

## Particularité du gameplay
- le joueur bouge des taquins sur l'interface utilisateur et doit avoir une bonne expérience/impression tactile
- le mouvement des taquins se reproduit sur les éléments de la scène sous-jacente  soit en 2D, en 2.5D ou en 3D (au choix)

<img src="https://github.com/jfc-babaoo/Test/raw/release/README.Assets/render.png" width="200">

## Compilation IOS et Android
le jeu doit compiler sous IOS et sous Android. Bien-sûr, le logo utilisé pour les taquins doit correspondre à la plate-forme ciblée.


# Réalisation du test
@author : Gautier Kasperek
## Architecture

### Présentation des scènes du jeu
Le projet se compose en deux scènes :
	- la scène "Home" affiche le logo du jeu, le meilleur score enregistré et un bouton pour lancer la partie (chargement de la seconde scène).
	- la scène "TaquinGame" et le jeu a proprement parler. Il  possède jeu de taquin, un bouton de retour au menu et un chronomètre permettant de compter les points.
La scène "TaquinGame" est composé de :
- Une *MainCamera*,
- Une *Directional Light*,
- 9 Palets (*Palet0, ... Palet9*) qui représentent les taquins à faire glisser pour résoudre le puzzle,
- Un *SceneController* permettant de naviguer entre les scènes,
- Un *Canvas* possèdant un Bouton et deux emplacements de *Text* (le chronometre et le message de victoire/défaite)
- Un EmptyGameObject *ScriptGameHandler* ayant pour principaux composant les scripts permettant la gestion du jeu.

### Présentation des scripts du jeu
Le Jeu est composé de 4 scripts :
- **InitializeMaterials** : Initialise l'apparence des *Palets* permettant de décider dynamiquement du style du jeu en fonction de la plateforme (logo Apple ou Android). Actuellement une valeur publique permet de choisir l'apparence du jeu avec les valeurs "Android" ou "Apple".
- **MaxSaver** : Singleton permettant le stockage, la sauvegarde et le chargement du meilleur score du joueur. Le meilleur score est calculé en fonction du chronomètre qui décremente. Plus le puzzle est résolut rapidement plus le score est haut.
- **TaquinGame** : Est la classe principale du projet. Elle rassemble la génération du Taquin, les mouvements des *Palets* et les conditions de victoire du jeu.
- **TeasingGameHomeSceneController** : Permet la navigation entre les scènes grâce aux fonctions "GoToGameScene" et "GoToHomeScene"

## Revue de code
Dans cette revue de code il sera rapidement expliciter comment la génération du jeu a été construite. Dans un premier temps, la fonction *CountInvert* prends une liste d'entier représentant l'ordre des taquins dans un jeu et compte le nombre d'inversion. Pour qu'un puzzle de taille 3 soit soluble le nombre d'inversions doit être pair. On compte une inversion lorsqu'un nombre **i** est positionné avant **j** mais **i > j**.  
```
   /*
    * count the number of inversion. Our Taquin is 3*3 so it needs to be pair
    */
    public int CountInvert(List<int> taquinArray)
    {
        int count = 0;

        // If i is before j but i is greater, it's an inversion
        for (int i = 0; i < this.size * this.size - 1; i++)
        {
            for (int j = i + 1; j < this.size * this.size; j++)
            {
                if (taquinArray[i] > taquinArray[j])
                {
                    count++;
                }
            }
        }

        return count;
    }
```
*isSolvable* Permet simplement de vérifier que le nombre d'inversion est pair. D'autres conditions pourraient s'ajouter ici mais on ne considère que les puzzles de taille 3 dans notre réalisation.
```
    /*
     * It's solvable if the inversion is even
     */
    public bool isSolvable(List<int> taquinArray)
    {
        return (this.CountInvert(taquinArray) % 2 == 0);
    }

    /*
     * Do 1 shuffle on taquin list
     */
    public List<int> ShuffleTaquin(List<int> taquin)
    {
        for (int j = 0; j < 100; j++)
        {
            // Randomly permuting index some tim
            for (int i = 0; i < taquin.Count; i++)
            {
                int temp = taquin[i];
                int randomIndex = UnityEngine.Random.Range(0, taquin.Count);
                taquin[i] = taquin[randomIndex];
                taquin[randomIndex] = temp;
            }
        }

        return taquin;
    }
```
Pour générer un taquin, on mélange l'ordre de notre liste jusqu'à trouver un puzzle soluble. Cela est rapide dans le cas d'un puzzle de taille 3 mais pourrait être plus long dans le cas d'un puzzle plus grand. Lorsque le puzzle est définit, on dispose les *Palets* en fonction d'une grille de coordonnées que l'on a définit *listOfPosition*. Enfin, il est retiré et supprimé le gameObject représentant le palet du centre que l'on ne souhaite pas dans la résolution.

```
    /*
     * Generate a solvable taquin
     * Create list of index :
     *      -> Shuffle the index until it's solvable ( odd grid so we need even number of invert)
     * Dispose palet according to our grid (listOfPosition)
     * Delete the palet 0
     * play game
     */
    public void generateTaquin()
    {
        // Initialize list
        List<int> taquinList = new List<int>();
        for (int i = 0; i < this.size * this.size; i++) taquinList.Add(i);

        // Shuffle it
        do
        {
            ShuffleTaquin(taquinList);
        } while (isSolvable(taquinList));

        // Display palets on the listOfPosition
        for(int t = 0; t < this.size * this.size; t++)
        {
            Palets[taquinList[t]].transform.position = this.listOfPostion[t];
        }

        // Remove then delete palet number 0
        for(int d = 0; d < this.size *this.size; d++)
        {
            if (taquinList[d] == 0)
            {
                GameObject toDelete = Palets[taquinList[d]];
                this.isEmpty[d] = true;
                Palets.Remove(toDelete);
                Destroy(toDelete.gameObject);
            }
        }
    }
```
Après cette revu de code, il est présenté les améliorations et les différents axes de développement du projet.


## Amélioration et axes de développement
 Pour ce qui est de l'amélioration de la structure dans un premier temps, il serait judicieux de refactorer la classe **TaquinGame**. En effet celle-ci est devenu relativement volumineuse et pourrait être séparé en différent module tels que la génération du jeu, la gestion du gameplay/déplacement et les différentes conditions d'arrêt. De plus, il serait également intéressant de ré-organiser le code de la partie déplacement de *Palets*, certaines fonctions plusieurs choses différentes ou certaines fonctionnalités sont séparés sans réél intérêt. Le code est fonctionnel mais gagnerait à être un petit peu clarifier.
 Une fois le code clarifier, il pourra être simplement étendu à des jeux de tailles N*N. Certains modules sont déjà implémentés de cette manière mais d'autres devront être réalisés comme le découpage dynamique de l'image du taquin.
 Enfin, n'ayant jamais réaliser de compilation sur mobile (iOS ou Android) je n'ai pas eu le temps de le réaliser.

## Annexes et sources
Wiki du jeu de taquin :
https://fr.wikipedia.org/wiki/Taquin

Résolution et génération de taquin :
https://www.geeksforgeeks.org/check-instance-15-puzzle-solvable/
https://www.geeksforgeeks.org/check-instance-8-puzzle-solvable/

[bestScore](https://github.com/GKasperek/TestTechnical/tree/main/README.Assets/bestScore.PNG")

<img src="https://github.com/GKasperek/TestTechnical/tree/main/README.Assets/newBestScore.PNG" width="200">

<img src="https://github.com/GKasperek/TestTechnical/tree/main/README.Assets/finished.PNG" width="200">

<img src="https://github.com/GKasperek/TestTechnical/tree/main/README.Assets/gameover.PNG" width="200">

<img src="https://github.com/GKasperek/TestTechnical/tree/main/README.Assets/win.PNG" width="200">
