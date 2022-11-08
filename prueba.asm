;Archivo: prueba.cpp
;Fecha: 08/11/2022 12:52:05 p. m.
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
	 cinco DW 0
	 x DW 0
	 y DW 0
	 i DW 0
	 j DW 0
	 k DW 0
doWhileInicio1:
MOV AX, i
PUSH AX
POP AX
CALL PRINT_NUM
PRINTN ""
PRINT ""
INC i
MOV AX, i
PUSH AX
MOV AX, 5
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE doWhileFin1:
JMP doWhileInicio1:
doWhileFin1:
PRINTN "Hola"
PRINTN " Me Llamo "
PRINT "Jair"
RET
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
DEFINE_SCAN_NUM
