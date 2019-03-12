;; 2.1 Buffer Name
;; ‘C-u C-x C-e’ causes the value returned to appear after the expression
(buffer-name)
(buffer-file-name)

;; 2.2 Getting Buffers
(current-buffer)
(other-buffer)

;; 2.3 Switching Buffers
(switch-to-buffer (other-buffer)) ; C-x b -> Recent invisible buf
(switch-to-buffer (other-buffer (current-buffer) t)) ; Recent visible buf

;; 2.4 Buffer Size and the Location of Point
(buffer-size)
(point)
(point-min)
(point-max)
