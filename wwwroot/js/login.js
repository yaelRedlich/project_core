const uri = '/User/Login';
let user = [];

const toSend = () => {
    let name = document.getElementById('name');
    let passWord = document.getElementById('passWord');
    login(name.value, passWord.value);
}

const login = async (name, passWord) => {
    user = {
        "UserId": 0, 
        "Username": name,
        "Password": passWord,
        "isAdmin": false
    };
    try {
        const response = await fetch(uri, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        });

        if (!response.ok) { 
            alert("Incorrect username or password!");
            return;
        }

        const text = await response.text(); 
        const cleanToken = text.replace(/"/g, ''); 

        if (!cleanToken) { 
            alert("Incorrect username or password!");
            return;
        } 
        sessionStorage.setItem('token', cleanToken); 
        let token= sessionStorage.getItem('token');
        let payload = JSON.parse(atob(token.split('.')[1])); 
        const isAdmin = payload.isAdmin; 
        if(isAdmin==='true')
            location.href="admin.html";
        else
            location.href = "book.html"; 

    } catch (error) {
        console.error('Login error', error);
        alert("Login error, please try again later.");
    }
}
