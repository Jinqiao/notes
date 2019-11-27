;; 10.1 Kill Ring Overview
;; The kill ring is a list of textual strings.


;; 10.2 The ‘kill-ring-yank-pointer’ Variable

;; kill-ring     kill-ring-yank-pointer
;;     |               |
;;     |      ___ ___  |     ___ ___      ___ ___
;;      ---> |   |   |  --> |   |   |    |   |   |
;;           |___|___|----> |___|___|--> |___|___|--> nil
;;             |              |            |
;;             |              |            |
;;             |              |             --> "yet more text"
;;             |              |
;;             |               --> "a different piece of text"
;;             |
;;              --> "some text"
