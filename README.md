BT-BR Only for now. (EN soon)


Funcionalidade:
Corrigir problema de audio "picotando" ou "craquelando" ao usar programas de manipulação de áudio como mixers customizados, filtros e dispositivos virtuais.

Comportamento:
-Manipular a afinidade e prioridade do processo nativo do windows AudioDG.exe.

Quando usamos qualquer programas que manipulam audio como "virtual cable" "virtual microphones" e outros semelhantes o Windows utiliza o subsistema AudioDG.exe para lidar com o tratamento de áudio em tempo real
Mas um problema que normalmente ocorre é a assincronicidade no uso dos nucleos da CPU, fazendo com que pacotes de audio cheguem de forma desodernada a stream de audio.

Um contorno encontrado é forçar o processo AudioDG.exe a utilizar apenas um dos núcleos do processador, o que força uma serialização dos dados tratados pelo mesmo.

Este programa permite de forma facil e com interface grafica não somente manitular o uso da CPU pelo AudioDG.exe como também a prioridade do mesmo.
E também permite que o usuário possa decidir qual núcleo do processador e qual a prioridade será aplicada ao mesmo de forma intuitiva.
