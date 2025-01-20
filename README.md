# PTFGym - Mobilna Aplikacija

"PTFGym" je mobilna aplikacija razvijena kao dio predmeta **Razvoj mobilnih aplikacija**, pružajući sveobuhvatno digitalno rješenje za ljubitelje fitnessa i aktivnog načina života. Cilj aplikacije je omogućiti korisnicima praćenje napretka, personalizaciju treninga sa trenerima i pojednostavljenje upravljanja članarinom kroz intuitivnu i brzu aplikaciju.

---

## Sadržaj
1. [Uvod](#uvod)
2. [Funkcionalnosti](#funkcionalnosti)
   - [Modul za članove](#modul-za-članove)
   - [Modul za trenere](#modul-za-trenere)
3. [Tehnologije](#tehnologije)
4. [UI/UX Dizajn](#uiux-dizajn)
5. [Instalacija](#instalacija)
6. [Korištenje](#korištenje)
7. [Zavisnosti](#zavisnosti)
8. [Primjeri](#primjeri)
9. [Autori](#autori)

---

## Uvod

"PTFGym" je osmišljena za današnji ubrzani način života, omogućavajući korisnicima da prate napredak u teretani, zakazuju treninge i surađuju sa trenerima putem modernog korisničkog interfejsa. Aplikacija eliminiše potrebu za fizičkim dolaskom u teretanu radi obnove članstva i pruža detaljne statistike korisnicima.

---

## Funkcionalnosti

### Modul za članove
Korisnici mogu:
- **Digitalno upravljanje članarinom**: Praćenje i plaćanje članarina online.
- **Praćenje napretka**: Vizualizacija rezultata treninga kroz detaljne statistike.
- **Grupni treninzi**: Pregled i brzo učlanjenje u grupne termine.
- **Rezervacije privatnih treninga**: Zakazivanje privatnih treninga sa trenerima.

### Modul za trenere
Trenerima omogućava:
- **Praćenje klijenata**: Unos i pregled napretka klijenata uz bilješke i preporuke.
- **Kalendar termina**: Prikaz svih zakazanih grupnih i privatnih treninga.
- **Upravljanje rezervacijama**: Potvrda i otkazivanje privatnih treninga.

---

## Tehnologije
- **Framework**: .NET 9.0 (MAUI - Multi-platform App UI)
- **Baza podataka**: SQLite
- **Jezici**: C#, XAML
- **Dizajn**: Moderan i intuitivan UI za sve nivoe korisnika.

---

## UI/UX Dizajn

PTFGym stavlja naglasak na jednostavnost i responzivan dizajn. Korisnički interfejs je osmišljen kako bi vodio korisnike intuitivno kroz funkcionalnosti aplikacije. Ključni UI elementi:

1. **Početni ekran**: Čist interfejs s opcijama za prijavu ili registraciju.
   - ![Početni ekran](#)
2. **Login/Registracija**: Siguran proces prijave i registracije.
   - ![Login/Registracija](#)
3. **Glavni ekran**: Centralna ploča za pristup funkcijama kao što su termini, napredak i članarina.
   - ![Glavni ekran](#)
4. **Stranica za termine**:
   - **Za članove**: Prikazuje sve dostupne grupne termine.
   - **Za trenere**: Prikazuje termine koje je trener kreirao uz opciju dodavanja novih termina.
   - ![Termini - Član](#)
   - ![Termini - Trener](#)
5. **Stranica za rezervacije**: Prikazuje rezervisane privatne treninge s opcijama za članove i trenere.
   - ![Stranica za rezervacije](#)
6. **Profil stranica**: Prikazuje informacije o korisniku s opcijom za odjavu.
   - ![Profil stranica](#)
7. **Stranica za članarinu**: Prikazuje detalje članarine i omogućava njeno produženje.
   - ![Stranica za članarinu](#)
8. **Admin ploča**: Centralizirana kontrolna ploča za upravljanje članovima, trenerima i terminima.
   - ![Admin ploča](#)

---

## Instalacija

1. Klonirajte repozitorij:
   ```bash
   git clone https://github.com/FarisB1/MAUI-Gym-App.git
2. Otvorite projekat u Visual Studio-u. 
3. Osigurajte da su sve zavisnosti preuzete preko NuGet Package Manager-a.
4. Izgradite i pokrenite aplikaciju na željenoj platformi (Android, iOS, ili Windows).


## Korištenje

1. Pokrenite aplikaciju.
2. Kreirajte nalog ili se prijavite kao admin, član ili trener.
3. Istražite funkcionalnosti:

    **Članovi:** Pratite napredak, prijavite se na grupne treninge i upravljajte članarinom.

    **Treneri:** Unosite napredak, upravljajte rezervacijama i kreirajte termine.

   **Administratori:** Upravljajte svim korisnicima i terminima.

## Zavisnosti

**.NET MAUI**

**SQLite** za čuvanje podataka

**NuGet paketi:**

    CommunityToolkit.Mvvm

    Microsoft.EntityFrameworkCore

    XAML Design Toolkit


## Primjeri

**Rad člana:**

    1. Prijavite se.

    2. Pregledajte dostupne termine i prijavite se jednim klikom.

    3. Pregledajte svoj napredak u sekciji "Napredak".

**Rad trenera:**

    1. Prijavite se kao trener.

    2. Pristupite stranici za termine i upravljajte rasporedom.

    3. Ažurirajte napredak za svoje klijente.

## Autori

**Faris Brkić**

**Ahmed Hasičić**