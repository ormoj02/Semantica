;Archivo: prueba.cpp
;Fecha: 05/11/2022 03:29:47 p. m.
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
	 x1 DW 0
	 x2 DW 0
	 y DW 0
	 i DW 0
	 j DW 0
	 a1 DW 0
MOV AX, 65535
PUSH AX
POP AX
MOV a, AX
