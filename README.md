![Screenshot](Captures/logo_padding.png)

## Resumé
Dual Disk est un jeu multijoueur de duels en 3D à la troisième personne s’inspirant de l’univers de Tron. Une partie se déroule en plusieurs manches dans une arène impliquant deux joueurs qui se battent avec des disques. Ces derniers, peuvent rebondir sur les murs un nombre limité de fois et enlever une vie lorsqu’ils touchent un joueur. Les joueurs quant à eux, peuvent esquiver les disques en courant, sautant ou en plongeant sur leur partie de terrain. Que le meilleur gagne !  
Lien du trailer: https://www.youtube.com/watch?v=fmGnnMaMJBk  
Lien de la version build : https://drive.google.com/drive/folders/1m_Vtzp8q3jgbk5vumHKVbOGmL__JmsvF?usp=sharing

## Règles
Une partie se déroule sur 5 manches. Un joueur remporte une manche lorsqu'il touche 3 fois l'adversaire ou qu'il le fait tomber de l'arène.  
Le joueur possède 3 types de disques:
- un disque normal.
- un disque spécial qui détruit les hexagones au sol.
- un disque spécial qui poursuit l'adversaire.

Chaque disque peut rebondir jusqu'à 5 fois. Un disque spécial peut etre utilisé toutes les 12 secondes. Le joueur peut accumuler 2 esquives, chaque esquive se rechargeant au bout de 5 secondes.

## Pour jouer
Dual Disk est un jeu multijoueur en 1v1. Il repose sur le principe de client-serveur. Le joueur peut décider de soit créer sa propre partie en cliquant sur "Host Game" dans le menu ou rejoindre une partie en cliquant sur "Join Game".
- Host: Pour pouvoir héberger une partie et autoriser un autre joueur à la rejoindre, il faut ouvirir le port 7777 de sa box. Pour cela se connecter à l'interface administration de sa box, puis ajouter une redirection de port comme ceci :
![Screenshot](Captures/ip.png)
Remplacer le champ IP de destination par votre adresse IPv4 disponible en lançant la commande ipconfig dans le terminal windows.

- Client: Pour rejoindre une partie, il suffit de récupérer l'adresse IP du joueur Host et la rentrer dans le champ texte "Adresse IP", puis de cliquer sur Join.
La partie se lancera automatiquement quelques secondes après l'arrivé du client.

## Dépendances du projet
Le projet est sous la version 2019.4.19f1 (LTS) de Unity. Voici les packages à télécharger pour faire fonctionner le programme:
- Cinemachine
- Core RP Library
- Post Processing
- TextMeshPro
- Universal RP

## Captures
![Screenshot](Captures/image6.png)
![Screenshot](Captures/image14.png)
![Screenshot](Captures/image15.png)
![Screenshot](Captures/image10.png)

