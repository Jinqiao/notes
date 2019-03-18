;; The key binding for ‘narrow-to-region’ is ‘C-x n n’
;; The key binding for ‘widen’ is ‘C-x n w’

;; 6.1 The ‘save-restriction’ Special Form

;; When you write the two ‘save- ...’ expressions in sequence, write
;; ‘save-excursion’ outermost.

;; 6.2 ‘what-line’
(defun what-line-old ()
  "Print the current line number (in the buffer) of point."
  (interactive)
  (save-restriction
    (widen)
    (save-excursion
      (beginning-of-line)
      (message "Line %d"
               (1+ (count-lines 1 (point)))))))


;; 6.3 Exercise with Narrowing

;; Write a function that will display the first 60 characters of the
;; current buffer, even if you have narrowed the buffer to its latter
;; half so that the first line is inaccessible.  Restore point, mark,
;; and narrowing.

(defun first-60-chars ()
  "print first 60 chars of buffer even narrowed"
  (interactive)
  (save-excursion
    (save-restriction
      (widen)
      (message (buffer-substring-no-properties 1 60))
      ))
  )

(first-60-chars)
