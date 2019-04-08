;; 1.1 Lisp Lists
;; In brief, a list is between parentheses, a string is between
;; quotation marks, a symbol looks like a word, and a number looks
;; like a number.

;; 1.2 Run a Program
;; A list in Lisp—any list—is a program ready to run.
;; If you run it the computer will do one of three things:
;; 1) do nothing except return to you the list itself;
;; 2) send you an error message;
;; 3) treat the first symbol in the list as a command to do something.

;; Single apostrophe before a list tells Lisp to do nothing with the list
;; other than take it as it is written (same for variable/symbol)

;; To evaluate a list -> place your cursor immediately after the right
;; hand parenthesis of a list then type ‘C-x C-e’
(+ 1 2)

;; 1.3 Generate an Error Message

;; 1.4 Symbol Names and Function Definitions

;; 1.5 The Lisp Interpreter

;; Complication
;; 1) variable
;; 2) special form
;; 3) macro
;; 4) evaluate inner list first
(+ 2 (+ 3 3))

;; Bype Compiling
;; Byte compiled code is usually stored in a file that ends with a ‘.elc’
;; extension rather than a ‘.el’ extension.

;; 1.6 Evaluation

;; 1.7 Variables
;; A symbol can have both a function definition and a value attached to
;; it at the same time.
;; Below is a variable, can be evaluated by 'C-x C-e'
fill-column


;; 1.8 Arguments
;; 1) Argument Data Type
;; Below use 2 strings arguments
(concat "abc" "def")
;; Below use both string and number as arguments
(substring "The quick brown fox jumped." 16 19)

;; 2) An Argument as the Value of a Variable or List
(+ 2 fill-column)
(concat "The " (number-to-string (+ 2 fill-column)) " red foxes.")

;; 3) Variable Number of Arguments
(+)
(+ 3)
(+ 3 4 5)

;; 4) Using the Wrong Type Object as an Argument
(+ 2 'hello)

;; 5) The ‘message’ Function
(message "This message appears in the echo area!")
(message "The name of this buffer is: %s." (buffer-name))
(message "The value of fill-column is %d." fill-column)
(message "There are %d %s in the office!"
         (- fill-column 14) "pink elephants")
(message "He saw %d %s"
         (- fill-column 32)
         (concat "red "
                 (substring
                  "The quick brown foxes jumped." 16 21)
                 " leaping."))


;; 1.9 Setting the Value of a Variable
;; 1) Using ‘set’
(set 'flowers '(rose violet daisy buttercup))
flowers
;; 2) Using ‘setq’
(setq carnivores '(lion tiger leopard))
(setq trees '(pine fir oak maple)
      herbivores '(gazelle antelope zebra))
;; 3) Counting
(setq counter 0)                ; Let’s call this the initializer.
(setq counter (+ counter 1))    ; This is the incrementer.
counter                         ; This is the counter.


;; 1.10 Summary
;; Lisp programs are made up of expressions, which are lists or single atoms.

;; Atoms are multi-character symbols, like ‘forward-paragraph’, single
;; character symbols like ‘+’, strings of characters between double
;; quotation marks, or numbers.


;; 1.11 Exercises
;; Generate an error message by evaluating an appropriate symbol that
;; is not within parentheses.
abc
;; Generate an error message by evaluating an appropriate symbol that
;; is between parentheses.
(abc)
;; Create a counter that increments by two rather than one.
(setq c 0)
(setq c (+ c 2))
c
;; Write an expression that prints a message in the echo area when evaluated
(message "Hello World %d" 123)
