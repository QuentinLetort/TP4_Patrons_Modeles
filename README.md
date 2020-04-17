# TP4 Patrons Modeles

Ce TP se compose de trois applications distinctes : une application principale qui crée des logs d’exécution et deux consommateurs (qui sont eux aussi des applications) qui les utilisent de manières différentes.

## Utilisation

### Lancer l'application d'écriture des logs dans un fichier

```sh
cd FileDump/
dotnet run [LOGFILE]
```
Cette application permet d'écrire les logs de sévérité Warning ou plus dans un fichier.

### Lancer l'application d'analyse de logs

```sh
cd LogAnalysis/
dotnet run
```
Cette application permet d'observer le nombre de logs par niveau de sévérité toutes les 10 secondes.
```console
Info : 3848 logs
Warning : 3658 logs
Error : 3738 logs
Critical : 3757 logs
```

### Lancer l'application d'émission des logs

```sh
cd LogEmitter/
dotnet run [NBLOGS]
```
Cette application permet de générer un nombre donnée de logs choisissant de manière aléatoire le niveau de sévérité.



