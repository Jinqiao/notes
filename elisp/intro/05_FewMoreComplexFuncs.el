;; 5.1 The Definition of ‘copy-to-buffer’
(defun copy-to-buffer (buffer start end)
  "Copy to specified buffer the text of the region.
It is inserted into that buffer, replacing existing text there.

When calling from a program, give three arguments:
BUFFER (or buffer name), START and END.
START and END specify the portion of the current buffer to be copied."
  (interactive "BCopy to buffer: \nr")
  (let ((oldbuf (current-buffer)))
    (with-current-buffer (get-buffer-create buffer)
      (barf-if-buffer-read-only)
      (erase-buffer)
      (save-excursion
        (insert-buffer-substring oldbuf start end)))))


;; 5.2 The Definition of ‘insert-buffer’
(defun insert-buffer (buffer)
  "Insert after point the contents of BUFFER.
Puts mark after the inserted text.
     BUFFER may be a buffer or a buffer name."
  (interactive "*bInsert buffer: ")
  (or (bufferp buffer)
      (setq buffer (get-buffer buffer)))
  (let (start end newmark)
    (save-excursion
      (save-excursion
        (set-buffer buffer)
        (setq start (point-min) end (point-max)))
      (insert-buffer-substring buffer start end)
      (setq newmark (point)))
    (push-mark newmark)))

;; The asterisk in interactive is for the situation when the current
;; buffer is a read-only buffer. If ‘insert-buffer’ is called when the
;; current buffer is read-only, a message to this effect is printed in
;; the echo area and the terminal may beep or blink at you

;; The purpose of the ‘or’ expression is to ensure that the argument
;; ‘buffer’ is bound to a buffer and not just the name of a buffer.

;; An ‘or’ function can have any number of arguments.  It evaluates
;; each argument in turn and returns the value of the first of its
;; arguments that is not ‘nil’.  Also, it does not evaluate any
;; subsequent arguments after returning the first non-‘nil’ value.


;; 5.3 Complete Definition of ‘beginning-of-buffer’

;; 5.3.1 Optional Arguments
;; In a function definition, if an argument
;; follows the keyword ‘&optional’, no value need be passed to that
;; argument when the function is called.

;; 5.5 ‘optional’ Argument Exercise
;; Write an interactive function with an optional argument that tests
;; whether its argument, a number, is greater than or equal to, or else,
;; less than the value of ‘fill-column’, and tells you which, in a message.
;; However, if you do not pass an argument to the function, use 56 as a
;; default value.

(defun is-larger-than-fill-column (&optional arg)
  "exercise 5.5"
  (interactive "P")
  (let ((value (if arg
                   (prefix-numeric-value arg)
                 56)))
    (if (> value fill-column)
        (message "%d is greater than fill-column" value)
      (if (= value fill-column)
          (message "%d is equal fill-column" value)
        (message "%d is less than fill-column" value))))
  )

(is-larger-than-fill-column 56)
(is-larger-than-fill-column 70)
(is-larger-than-fill-column 77)
