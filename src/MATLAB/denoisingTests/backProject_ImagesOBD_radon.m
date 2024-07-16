
nProjections =100;%length(origImages);

step=(length(origImages))/nProjections;

angleStep=360.0/(nProjections+1);

s = size(origImages{2});


sinoGram = zeros([s(2) nProjections]);
figure;
imagesc( origImages{1});
colormap gray;
for I=1:nProjections
    sI=round( (I-1)*step+1)
    sI2 = mod(sI+ length(origImages)/2-1,length(origImages))+1
    
    projection =doOBDFilter( origImages{sI}, origImages{sI2});
    
    imagesc(projection);drawnow;
    filteredImagesOBD{I}=projection;
    
    s2=size(projection);
    startI=round( (size(projection,2) - s(2))/2);
    
    projection = projection(startI:end-startI,round(s2(2)/2))';
    sinoGram(:,I)=projection(:);
end

thetas = ((1:nProjections)-1)*angleStep;

reconImage=iradon(sinoGram,thetas,'linear','Ram-Lak',1,s(2));

figure;imagesc(reconImage(70:end-70,70:end-70));colormap gray;
D=reconImage;


