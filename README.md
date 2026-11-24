# GK1-P2 – Powierzchnie Béziera

## Obsługa

Przeciąganie lewym przyciskiem myszy po obszarze rysowania obraca scenę: ruch w poziomie wokół osi y ekranu, ruch w pionie wokół osi x ekranu.
Kółko myszy obraca scenę wokół osi z (5° na ząbek).
Lewy przycisk na punkcie kontrolnym krzywej światła przeciąga ten punkt (gdy włączone jest „Show light curve points”).
Panel po prawej: dokładność triangulacji, wybór rysowanych elementów, kd, ks, m, kolor światła, wysokość z i animacja światła, kolor obiektu wybranej powierzchni (stały albo tekstura) oraz mapa normalnych.

Skróty klawiaturowe: F1 – dokumentacja, Spacja – zatrzymanie/wznowienie animacji światła, R – powrót do widoku startowego.

## Obrót myszą

Orientacja sceny to jedna macierz obrotu R. Nie pamiętamy kątów, więc kąty obrotu nie mają ograniczeń.
Przy każdym ruchu myszy liczymy przesunięcie (dx, dy) od poprzedniego zdarzenia (a nie od początku przeciągania) i tworzymy z niego dwa małe obroty: Ry(0.01 · dx) i Rx(0.01 · dy), czyli 0.01 radiana na piksel.
Przyrost składamy z dotychczasową orientacją jako R = Rprzyrost · R. Mnożenie z lewej strony sprawia, że przyrost działa na już obróconą scenę, czyli zawsze wokół osi ekranu, a nie osi obiektu.
Dzięki temu przeciągnięcie w prawo zawsze przesuwa przednią część sceny w prawo, a w dół – w dół, niezależnie od dotychczasowego obrotu (jakby chwycić kulę za przednią ściankę).
Po każdym kroku macierz jest ortonormalizowana (Gram-Schmidt), żeby błędy zaokrągleń nie zniekształcały sceny.
W każdej klatce punkty i wektory obracamy zawsze z wartości oryginalnych: P' = R · P, tak samo Pu, Pv i N (N potem normalizujemy).

## Model danych

Scena to lista powierzchni. Każda ma 16 punktów kontrolnych, siatkę trójkątów, własny kolor i teksturę.
Siatka to lista trójkątów, trójkąt to trzy wierzchołki, a wierzchołek trzyma u, v oraz P, Pu, Pv i N przed obrotem i po obrocie. Sąsiednie trójkąty współdzielą te same wierzchołki.

## Triangulacja

Kwadrat parametrów [0, 1] × [0, 1] dzielimy na steps × steps kratek, a każdą kratkę przekątną na dwa trójkąty.
W wierzchołkach liczymy P, Pu i Pv ze wzorów z wielomianami Bernsteina, a N = Pu × Pv.

## Wypełnianie

Trójkąty wypełniamy algorytmem scan-line z sortowaniem wierzchołków: gdy scan-linia mija wierzchołek, krawędź do sąsiada niżej dodajemy do AET, a krawędź do sąsiada wyżej usuwamy. x krawędzi przesuwamy przyrostowo o dx/dy na wiersz.
Dla każdego piksela liczymy współrzędne barycentryczne i interpolujemy z, N, Pu, Pv oraz u, v.
Z-bufor (większe z jest bliżej obserwatora) odrzuca zasłonięte piksele, zanim policzymy ich kolor.

## Oświetlenie

I = kd · IL · IO · cos(N, L) + ks · IL · IO · cos^m(V, R), osobno dla R, G i B, w zakresie 0–1, a wynik przycinamy do 255.
V = [0, 0, 1], R = 2⟨N, L⟩N − L, ujemne cosinusy zamieniamy na 0.
Gdy normalna patrzy od obserwatora (Nz < 0), odwracamy ją, więc oświetlone są obie strony powierzchni.

## Źródło światła

Światło porusza się po zamkniętej krzywej Béziera stopnia 6 (punkty V0–V5, a V6 = V0) na wysokości z ustawianej suwakiem.
Ruch zależy od czasu, a nie od liczby klatek (jedno okrążenie na 10 s). Światło nie obraca się razem ze sceną.

## Kolor obiektu i mapa normalnych

Kolor obiektu to stały kolor albo tekstura, osobno dla każdej powierzchni. Teksturę próbkujemy w punkcie (u, v).
Mapa normalnych: wektor z tekstury to N_tekstury = 2 · kolor − 1, a nowa normalna to M · N_tekstury, gdzie M = [Pu, Pv, N] (kolumny, Pu i Pv znormalizowane).

## Plik z powierzchniami

Data/surfaces.txt jest wczytywany przy starcie: po 16 wierszy „x y z” na każdą powierzchnię.
