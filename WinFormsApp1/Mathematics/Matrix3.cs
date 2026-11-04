using System.Numerics;

namespace WinFormsApp1.Mathematics
{
    /// <summary>
    /// Macierz 3 x 3 dla wektorów KOLUMNOWYCH, dokładnie jak na wykładzie i w treści zadania:
    ///
    ///         [ r11 r12 r13 ]
    ///     R = [ r21 r22 r23 ]        P' = R · P
    ///         [ r31 r32 r33 ]
    ///
    /// Z biblioteki bierzemy tylko Vector3 (punkt / wektor x, y, z). Mnożenia piszemy sami.
    /// </summary>
    public readonly struct Matrix3
    {
        // Nazwy pól jak na wykładzie: rWK = wiersz W, kolumna K.
        public readonly float R11, R12, R13;
        public readonly float R21, R22, R23;
        public readonly float R31, R32, R33;

        public Matrix3(
            float r11, float r12, float r13,
            float r21, float r22, float r23,
            float r31, float r32, float r33)
        {
            R11 = r11; R12 = r12; R13 = r13;
            R21 = r21; R22 = r22; R23 = r23;
            R31 = r31; R32 = r32; R33 = r33;
        }

        /// <summary>Macierz jednostkowa: brak obrotu.</summary>
        public static readonly Matrix3 Identity = new Matrix3(
            1, 0, 0,
            0, 1, 0,
            0, 0, 1);

        /// <summary>Macierz z trzech kolumn: [a b c]. (Przyda się np. dla M = [Pu, Pv, N].)</summary>
        public static Matrix3 FromColumns(Vector3 a, Vector3 b, Vector3 c) => new Matrix3(
            a.X, b.X, c.X,
            a.Y, b.Y, c.Y,
            a.Z, b.Z, c.Z);

        public Vector3 Column1 => new Vector3(R11, R21, R31);
        public Vector3 Column2 => new Vector3(R12, R22, R32);
        public Vector3 Column3 => new Vector3(R13, R23, R33);

        // ------------------------------------------------------------------
        //  Macierze obrotu z treści zadania (kąt w radianach)
        // ------------------------------------------------------------------

        /// <summary>
        ///          [ 1    0       0    ]
        /// Rx(θ) =  [ 0  cos θ  −sin θ  ]
        ///          [ 0  sin θ   cos θ  ]
        /// </summary>
        public static Matrix3 RotationX(float angle)
        {
            float c = MathF.Cos(angle);
            float s = MathF.Sin(angle);
            return new Matrix3(
                1, 0, 0,
                0, c, -s,
                0, s, c);
        }

        /// <summary>
        ///          [  cos θ  0  sin θ ]
        /// Ry(θ) =  [    0    1    0   ]
        ///          [ −sin θ  0  cos θ ]
        /// </summary>
        public static Matrix3 RotationY(float angle)
        {
            float c = MathF.Cos(angle);
            float s = MathF.Sin(angle);
            return new Matrix3(
                c, 0, s,
                0, 1, 0,
                -s, 0, c);
        }

        /// <summary>
        ///          [ cos θ  −sin θ  0 ]
        /// Rz(θ) =  [ sin θ   cos θ  0 ]
        ///          [   0       0    1 ]
        /// </summary>
        public static Matrix3 RotationZ(float angle)
        {
            float c = MathF.Cos(angle);
            float s = MathF.Sin(angle);
            return new Matrix3(
                c, -s, 0,
                s, c, 0,
                0, 0, 1);
        }

        // ------------------------------------------------------------------
        //  Mnożenia
        // ------------------------------------------------------------------

        /// <summary>
        /// Macierz razy wektor (P' = R · P), wzór z treści zadania:
        ///   x' = r11 x + r12 y + r13 z
        ///   y' = r21 x + r22 y + r23 z
        ///   z' = r31 x + r32 y + r33 z
        /// </summary>
        public static Vector3 operator *(Matrix3 r, Vector3 p)
        {
            float x = r.R11 * p.X + r.R12 * p.Y + r.R13 * p.Z;
            float y = r.R21 * p.X + r.R22 * p.Y + r.R23 * p.Z;
            float z = r.R31 * p.X + r.R32 * p.Y + r.R33 * p.Z;
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Macierz razy macierz (C = A · B): element C[w, k] = wiersz w z A "razy" kolumna k z B.
        /// Przy wektorach kolumnowych A · B oznacza: najpierw obrót B, potem obrót A.
        /// </summary>
        public static Matrix3 operator *(Matrix3 a, Matrix3 b)
        {
            return new Matrix3(
                a.R11 * b.R11 + a.R12 * b.R21 + a.R13 * b.R31,
                a.R11 * b.R12 + a.R12 * b.R22 + a.R13 * b.R32,
                a.R11 * b.R13 + a.R12 * b.R23 + a.R13 * b.R33,

                a.R21 * b.R11 + a.R22 * b.R21 + a.R23 * b.R31,
                a.R21 * b.R12 + a.R22 * b.R22 + a.R23 * b.R32,
                a.R21 * b.R13 + a.R22 * b.R23 + a.R23 * b.R33,

                a.R31 * b.R11 + a.R32 * b.R21 + a.R33 * b.R31,
                a.R31 * b.R12 + a.R32 * b.R22 + a.R33 * b.R32,
                a.R31 * b.R13 + a.R32 * b.R23 + a.R33 * b.R33);
        }

        // ------------------------------------------------------------------
        //  Naprawianie macierzy obrotu
        // ------------------------------------------------------------------

        /// <summary>
        /// Macierz obrotu ma kolumny o długości 1, prostopadłe do siebie.
        /// Po tysiącach mnożeń błędy zaokrągleń float trochę to psują (scena zaczęłaby się
        /// rozciągać lub kurczyć). Ta metoda przywraca te własności (metoda Grama-Schmidta):
        ///   1. kolumna 1 -> skracamy/wydłużamy do długości 1,
        ///   2. kolumna 2 -> usuwamy z niej część równoległą do kolumny 1 i normalizujemy,
        ///   3. kolumna 3 -> liczymy od nowa jako iloczyn wektorowy kolumn 1 i 2
        ///      (wtedy jest prostopadła do obu i układ zostaje prawoskrętny).
        /// </summary>
        public Matrix3 Orthonormalized()
        {
            Vector3 c1 = Vector3.Normalize(Column1);

            Vector3 c2 = Column2 - Vector3.Dot(Column2, c1) * c1;
            c2 = Vector3.Normalize(c2);

            Vector3 c3 = Vector3.Cross(c1, c2);

            return FromColumns(c1, c2, c3);
        }
    }
}
