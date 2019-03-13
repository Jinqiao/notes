;; An Aside about Primitive Functions
;; All functions are defined in terms of other functions, except for a few
;; “primitive” functions that are written in the C programming language.

;; Unless you investigate, you won’t know whether an already-written
;; function is written in Emacs Lisp or C.


;; 3.1 The ‘defun’ Macro
;; 1) The name of the function
;; 2) A list of the arguments that will be passed to the function
;; 3) Documentation describing the function
;; 4) Optionally, an expression to make the function interactive
;; 5) The code that instructs the computer what to do

;; Template
(defun FUNCTION-NAME (ARGUMENTS...)
  "OPTIONAL-DOCUMENTATION..."
  (interactive ARGUMENT-PASSING-INFO)     ; optional
  BODY...)

;; Example: multiply by 7
(defun multiply-by-seven (number)
  "Multiply NUMBER by seven."
  (* 7 number))

(multiply-by-seven 3)


;; 3.2 Install a Function Definition


;; 3.3 Make a Function Interactive
;; ‘C-u’ and a number + ‘M-x multiply-by-seven’ + <RET>
(defun multiply-by-seven (number)       ; Interactive version.
  "Multiply NUMBER by seven."
  (interactive "p")
  (message "The result is %d" (* 7 number)))

;; A “prefix argument” is passed to an interactive function by typing
;; the <META> key followed by a number, for example, ‘M-3 M-e’, or by
;; typing ‘C-u’ and then a number, for example, ‘C-u 3 M-e’ (if you type
;; ‘C-u’ without a number, it defaults to 4).


;; 3.4 Different Options for ‘interactive’
;; a function with two or more arguments can have information passed to
;; each argument by adding parts to the string that follows ‘interactive’.
;; When you do this, the information is passed to each argument in the
;; same order it is specified in the ‘interactive’ list.
;; In the string, each part is separated from the next part by a ‘\n’,
;; which is a newline.  For example, you can follow ‘p’ with a ‘\n’
;; and an ‘cZap to char: ’.
(defun NAME-OF-FUNCTION (arg char)
  "DOCUMENTATION..."
  (interactive "p\ncZap to char: ")
  BODY-OF-FUNCTION...)


;; 3.5 Install Code Permanently
;; 1) .emacs file
;; 2) load file
;; 3) site-init.el


;; 3.6 ‘let’
;; variables created by a ‘let’ expression retain their value
;; _only_ within the ‘let’ expression itself.

;; After ‘let’ has created and bound the variables, it executes
;; the code in the body of the ‘let’, and returns the value of the last
;; expression in the body, as the value of the whole ‘let’ expression.

;; 3.6.1 The Parts of a ‘let’ Expression
(let ((VARIABLE VALUE)
      (VARIABLE VALUE)
      ...)
  BODY...)

;; a varlist might look like this: ‘(thread (needles 3))’.  In
;; this case, in a ‘let’ expression, Emacs binds the symbol ‘thread’ to an
;; initial value of ‘nil’, and binds the symbol ‘needles’ to an initial
;; value of 3.

;; 3.6.2 Sample ‘let’ Expression
(let ((zebra "stripes")
      (tiger "fierce"))
  (message "One kind of animal has %s and another is %s."
           zebra tiger))

;; 3.6.3 Uninitialized Variables in a ‘let’ Statement
(let ((birch 3)
      (pine )
      fir
      (oak 'some))
  (message
   "Here are %d variables with %s, %s, and %s value."
   birch pine fir oak))


;; 3.7 The ‘if’ Special Form
(if TRUE-OR-FALSE-TEST
    ACTION-TO-CARRY-OUT-IF-TEST-IS-TRUE)

(if (> 5 4)                             ; if-part
    (message "5 is greater than 4!"))   ; then-part

(defun type-of-animal (characteristic)
  "Print message in echo area depending on CHARACTERISTIC.
     If the CHARACTERISTIC is the string \"fierce\",
     then warn of a tiger."
  (if (equal characteristic "fierce")
      (message "It is a tiger!")))

(type-of-animal "fierce")
(type-of-animal "striped")


;; 3.8 If–then–else Expressions
(if TRUE-OR-FALSE-TEST
    ACTION-TO-CARRY-OUT-IF-THE-TEST-RETURNS-TRUE
  ACTION-TO-CARRY-OUT-IF-THE-TEST-RETURNS-FALSE)

(defun type-of-animal (characteristic)  ; Second version.
  "Print message in echo area depending on CHARACTERISTIC.
     If the CHARACTERISTIC is the string \"fierce\",
     then warn of a tiger; else say it is not fierce."
  (if (equal characteristic "fierce")
      (message "It is a tiger!")
    (message "It is not fierce!")))

(type-of-animal "fierce")
(type-of-animal "striped")


;; 3.9 Truth and Falsehood in Emacs Lisp
;; In Emacs Lisp, the symbol ‘nil’ has two meanings.  First, it means
;; the empty list.  Second, it means false and is the value returned when a
;; true-or-false-test tests false.

;; As far as the Lisp interpreter is concerned, ‘()’ and ‘nil’ are the same.
;; Any value that is not ‘nil’—is not the empty list—is considered true.

(if 4
    'true
  'false)

(if nil
    'true
  'false)

;; Incidentally, if some other useful value is not available for a test
;; that returns true, then the Lisp interpreter will return the symbol ‘t’
;; for true.  For example, the expression ‘(> 5 4)’ returns ‘t’ when
;; evaluated, as you can see by evaluating it in the usual way:
(> 5 4)

;; On the other hand, this function returns ‘nil’ if the test is false.
(> 4 5)


;; 3.10 ‘save-excursion’
;; It saves the location of point, executes the body of the function, and
;; then restores point to its previous position if its location was changed
(save-excursion
  FIRST-EXPRESSION-IN-BODY
  SECOND-EXPRESSION-IN-BODY
  THIRD-EXPRESSION-IN-BODY
  ...
  LAST-EXPRESSION-IN-BODY)

;; ‘C-x C-x’ (‘exchange-point-and-mark’) cause the cursor to jump to the
;; mark and set the mark to be the previous position of point

;; C-<SPC> C-<SPC>
;; Set the mark, pushing it onto the mark ring, without activating it.
;; C-u C-<SPC>
;; Move point to where the mark was, and restore the mark from the ring
;; of former marks.


;; 3.12 Exercises
;; • Write a non-interactive function that doubles the value of its
;; argument, a number.  Make that function interactive.

(defun double (n)
  "n * 2"
  (interactive "P")
  (* n 2)
  (message "ok")
  )

(double 12)


;; • Write a function that tests whether the current value of
;; ‘fill-column’ is greater than the argument passed to the function,
;; and if so, prints an appropriate message.
