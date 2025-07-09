    
from fastapi import FastAPI, Query
from fastapi.middleware.cors import CORSMiddleware
import pandas as pd

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:3000"],  
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

csv_path = "fra_cleaned.csv"
df = pd.read_csv(csv_path, encoding="latin1", sep=";")

accord_translations = {
    "rose": "rosa",
    "citrus": "FAMILIA CÍTRICA",
    "fruity": "FAMILIA FRUTAL",
    "aromatic": "FAMILIA AROMÁTICO",
    "white floral": "floral blanco",
    "woody": "FAMILIA AMADERADO",
    "powdery": "FAMILIA EMPOLVADO",
    "leather": "cuero",
    "green": "verde",
    "rubber": "caucho",
    "floral": "FAMILIA FLORAL",
    "ozonic": "ozónico",
    "vinyl": "vinilo",
    "musky": "FAMILIA ALMIZCLADO",
    "yellow floral": "floral amarillo",
    "earthy": "FAMILIA TERROSO",
    "warm spicy": "especiado cálido",
    "fresh spicy": "especiado fresco",
    "fresh": "fresco",
    "sweet": "dulce",
    "amber": "FAMILIA ÁMBAR",
    "vanilla": "vainilla",
    "tropical": "tropical",
    "lavender": "lavanda",
    "almond": "almendra",
    "violet": "violeta",
    "iris": "iris",
    "cherry": "cereza",
    "aquatic": "acuático",
    "aldehydic": "FAMILIA ALDEHÍDICO",
    "animalic": "animalico",
    "oud": "oud (madera de agar)",
    "marine": "FAMILIA MARINO",
    "metallic": "metálico",
    "lactonic": "lactónico",
    "coffee": "café",
    "tuberose": "nardo",
    "caramel": "caramelo",
    "smoky": "FAMILIA AHUMADO",
    "coconut": "coco",
    "soft spicy": "especiado suave",
    "patchouli": "pachulí",
    "sand": "arena",
    "anis": "anís",
    "mossy": "musgoso",
    "honey": "miel",
    "mineral": "mineral",
    "chocolate": "chocolate",
    "cacao": "cacao",
    "coca-cola": "coca-cola",
    "tobacco": "tabaco",
    "savory": "salado/sabroso",
    "plastic": "plástico",
    "herbal": "FAMILIA HERBAL",
    "nutty": "nuez/nuez moscada",
    "soapy": "jabonoso",
    "balsamic": "balsámico",
    "salty": "salado",
    "asphault": "asfalto",
    "champagne": "champán",
    "beeswax": "cera de abejas",
    "cinnamon": "canela",
    "whiskey": "whisky",
    "rum": "ron",
    "industrial glue": "pegamento industrial",
    "oriental": "oriental",
    "camphor": "FAMILIA ALCANFORADO",
    "hot iron": "hierro caliente",
    "cannabis": "cannabis",
    "vodka": "vodka",
    "clay": "arcilla",
    "wine": "vino",
    "brown scotch tape": "cinta adhesiva marrón",
    "spicy": "FAMILIA ESPECIADO",
    "gourmand": "FAMILIA GOURMAND",
    "oily": "aceitoso",
    "paper": "papel",
    "conifer": "conífero",
    "bitter": "amargo",
    "alcohol": "alcohol",
    "sour": "agrio"
}


@app.get("/api/perfume")
def search_perfume(name: str = Query(..., alias="q")):
    normalized_name = name.replace(" ", "-").lower()
    result = df[df["Perfume"].str.lower().str.contains(normalized_name, na=False)]

    if result.empty:
        return {"message": "Perfume not found", "notes": []}
    
    perfume = result.iloc[0]  # Tomamos el primero que coincida
    notes = [
        perfume.get("mainaccord1"),
        perfume.get("mainaccord2"),
        perfume.get("mainaccord3")
    ]
    
    translated_notes = [accord_translations.get(n.lower(), n) for n in notes if pd.notna(n)]
    return {
        "perfume": perfume["Perfume"],
        "brand": perfume["Brand"],
        #"notes": [n for n in notes if pd.notna(n)]
        "notes": translated_notes
    }


    
