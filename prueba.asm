;Archivo: prueba.cpp
;Fecha: 25/10/2022 09:53:01 a. m.
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
	 x DW 0
	 y DW 0
	 i DW 0
	 j DW 0
MOV AX, 61
PUSH AX
POP AX
MOV y, AX
MOV AX, 60
PUSH AX
MOV AX, 61
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if1
MOV AX, 10
PUSH AX
POP AX
MOV x, AX
if1:
RET
