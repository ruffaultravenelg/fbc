# Flowbyte Compiler (fbc)

FBC est un compilateur de fichier binaire FlowByte, écrit en VB.NET. Les fichiers binaires générés peuvent être interprétés via l'interpréteur [FBI](https://github.com/ruffaultravenelg/fbi).

## Compilation du code source
Copier le dépôt :
```sh
git clone https://github.com/ruffaultravenelg/fbc.git
cd fbc
```

Sous Windows, le projet peut être compilé via Visual Studio ou le fichier `make.bat`.
```bash
make
```
> Les différentes architectures visées peuvent être sélectionnées dans ce même fichier.

## Utilisation
Pour compiler un fichier source FlowByte :
```sh
fbc [filename]
```

Pour voir les options disponibles :
```sh
fbc --help
```

## Contenu du fichier source
Un fichier source FlowByte contient principalement une liste de fonctions, chaque fonction contenant des instructions.
```asm
def main
    mov 0, 'a'          ; Déplace la valeur du caractère 'a' dans le registre 0
    :loop
    arg $0              ; Pousse la valeur du registre 0 en argument
    int PUTC            ; Appelle l'interruption PUTC qui affiche le caractère correspondant au code ASCII passé en argument.
    equ $0, 'z'         ; Vérifie si la valeur du registre 0 est égale à la valeur de 'z'
    jmpif end, ?ret     ; Si la comparaison est valide, alors saute à :end et termine le programme
    inc 0               ; Sinon, incrémente de 1 le registre 0
    jmp loop            ; Saute au label loop pour repartir dans une itération
    :end
```

---

Chaque fonction possède ses propres "registres" de 32 bits, chacun peut être accédé avec $n où n est le registre. Avant d'appeler une fonction (`call`) ou une interruption (`int`), il est nécessaire de passer les arguments nécessaires via l'utilisation de l'instruction `arg`. Enfin, une valeur peut être retournée via le registre global `?ret`, modifié via l'instruction `retval`.

Création et utilisation d'une fonction `getBirthYear` qui prend en argument l'âge et renvoie la date de naissance :
```asm
def main
    arg 19
    call getBirthYear

    arg ?ret
    int PUTI

def getBirthYear 1
    sub 2025, $0
    retval ?ret
```

---

Enfin, voici un exemple de calcul de la suite de Fibonacci par récursivité. Une fonction `printlist` permettra d'écrire tous les nombres de la liste jusqu'au nombre passé en argument via une boucle.
```asm
def main
    arg 10
    call printlist

def printlist 1
    mov 1, 0
    :loop
    
    arg $1
    call fibonacci
    
    arg ?ret
    int PUTI
    
    arg '\n'
    int PUTC
    
    equ $1, $0
    jmpif endloop, ?ret
    
    inc 1
    jmp loop
    :endloop

def fibonacci 1
    lt $0, 0
    jmpif ret0, ?ret

    equ $0, 1
    jmpif ret1, ?ret

    sub $0, 1
    arg ?ret
    call fibonacci
    mov 1, ?ret

    sub $0, 2
    arg ?ret
    call fibonacci
    mov 2, ?ret

    add $1, $2
    retval ?ret
    ret    

    :ret0
    retval 0
    ret

    :ret1
    retval 1
    ret
```

Sortie :
```
0
1
1
2
3
5
8
13
21
34
55
```

Compilation :
```sh
fbc fibo.txt fibo
```

Execution :
```sh
fb fibo
```