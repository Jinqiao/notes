;; 8.1 ‘zap-to-char’
(defun zap-to-char (arg char)
  "Kill up to and including ARG'th occurrence of CHAR.
     Case is ignored if `case-fold-search' is non-nil in the current buffer.
     Goes backward if ARG is negative; error if CHAR not found."
  (interactive "p\ncZap to char: ")
  (if (char-table-p translation-table-for-input)
      (setq char (or (aref translation-table-for-input char) char)))
  (kill-region (point) (progn
                         (search-forward (char-to-string char)
                                         nil nil arg)
                         (point))))

;; (search-forward "TARGET-STRING"
;;                 LIMIT-OF-SEARCH
;;                 WHAT-TO-DO-IF-SEARCH-FAILS
;;                 REPEAT-COUNT)

;; ‘progn’ is a special form that causes each of its arguments to be
;; evaluated in sequence and then returns the value of the last one


;; 8.2 ‘kill-region’
(condition-case
    VAR
    BODYFORM
  ERROR-HANDLER...)

;; A ‘when’ expression is an ‘if’ without an else clause
;; The ‘unless’ macro is an ‘if’ without a then clause


;; 8.3 ‘copy-region-as-kill’
(defun copy-region-as-kill (beg end)
  "Save the region as if killed, but don't kill it.
     In Transient Mark mode, deactivate the mark.
     If `interprogram-cut-function' is non-nil, also save the text for a window
     system cut and paste."
  (interactive "r")
  (if (eq last-command 'kill-region)
      (kill-append (filter-buffer-substring beg end) (< end beg))
    (kill-new (filter-buffer-substring beg end)))
  (if transient-mark-mode
      (setq deactivate-mark t))
  nil)


;; 8.4 Digression into C


;; 8.5 Initializing a Variable with ‘defvar’
;; It is unlike ‘setq’ in two ways:
;; First, it only sets the value of the variable if the variable does
;; not already have a value.  Second, ‘defvar’ has a documentation
;; string.

;; Seeing the Current Value of a Variable -> 'C-h v'


;; 8.7 Searching Exercises
;; • Write an interactive function that searches for a string.  If the
;; search finds the string, leave point after it and display a message
;; that says “Found!”.  (Do not use ‘search-forward’ for the name of
;; this function; if you do, you will overwrite the existing version
;; of ‘search-forward’ that comes with Emacs.  Use a name such as
;; ‘test-search’ instead.)
(defun test-search (text)
  "ex 8.7.1"
  (interactive "MSearch Text: ")
  (let ((p1 (point)))
    (search-forward text)
    (unless (= p1 (point))
      (message "Found!"))))


;; • Write a function that prints the third element of the kill ring in
;; the echo area, if any; if the kill ring does not contain a third
;; element, print an appropriate message.
(defun third-of-kill-ring ()
  "get the third of kill-ring"
  (interactive)
  (setq third (nth 3 kill-ring))
  (if third
      (message "the third element of kill-ring is %s" third)
    (message "kill ring is too short")))

(third-of-kill-ring)
