;Archivo: prueba.cpp
;Fecha: 28/10/2022 09:42:00 a. m.
#make_COM#
include emu8086.inc
ORG 100h
;Variables: 
	 area DW 0
	 radio DW 0
	 pi DW 0
	 resultado DW 0
	 a DW 0
	 d DW 0
	 altura DW 0
	 k DW 0
	 l DW 0
	 x DW 0
	 y DW 0
	 i DW 0
	 j DW 0
MOV AX, 255
PUSH AX
POP AX
MOV y, AX
PRINTN "Introduce la altura de la piramide: "
CALL SCAN_NUM
MOV altura, CX
MOV AX, 2
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE if1
inicioFor0:
POP AX
MOV i, AX
MOV AX, 0
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE 
MOV AX, 0
PUSH AX
POP AX
MOV j, AX
POP BX
POP AX
SUB AX, BX
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
MOV AX, 2
PUSH AX
POP BX
POP AX
MOV AX, 0
PUSH AX
POP BX
