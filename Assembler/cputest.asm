SECTION "Header", ROM0[$100]	
SECTION "Game code", ROM0

Start:		ld sp,$1000		; setup stack pointer
			ld a,10			; load 10 into a
			ld b,3			; load 3 into b
loop:		dec b			; decrement b
			jp z, end		; if b is 0, jump to the end
			add a			; add a to itself
			jp loop			; jump to the top of the multiply loop
end:		halt			; end
